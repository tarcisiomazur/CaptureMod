using System;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Mixer;
using NAudio.Wave;

namespace CaptureMod.Voice
{
    public class MyWaveInEvent : IWaveIn, IDisposable
    {
        private readonly AutoResetEvent callbackEvent;
        private readonly SynchronizationContext syncContext;
        private IntPtr waveInHandle;
        private volatile CaptureState captureState;
        private WaveInBuffer[] buffers;

        public event EventHandler<WaveInEventArgs> DataAvailable;

        public event EventHandler<StoppedEventArgs> RecordingStopped;

        public MyWaveInEvent()
        {
            this.callbackEvent = new AutoResetEvent(false);
            this.syncContext = SynchronizationContext.Current;
            this.DeviceNumber = 0;
            this.WaveFormat = new WaveFormat(8000, 16, 1);
            this.BufferMilliseconds = 100;
            this.NumberOfBuffers = 3;
            this.captureState = CaptureState.Stopped;
        }

        public static int DeviceCount => WaveInterop.waveInGetNumDevs();

        public static WaveInCapabilities GetCapabilities(int devNumber)
        {
            WaveInCapabilities waveInCaps = new WaveInCapabilities();
            int waveInCapsSize = Marshal.SizeOf<WaveInCapabilities>(waveInCaps);
            MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr) devNumber, out waveInCaps, waveInCapsSize),
                "waveInGetDevCaps");
            return waveInCaps;
        }

        public int BufferMilliseconds { get; set; }

        public int NumberOfBuffers { get; set; }

        public int DeviceNumber { get; set; }

        private void CreateBuffers()
        {
            int bufferSize = this.BufferMilliseconds * this.WaveFormat.AverageBytesPerSecond / 1000;
            if (bufferSize % this.WaveFormat.BlockAlign != 0)
                bufferSize -= bufferSize % this.WaveFormat.BlockAlign;
            this.buffers = new WaveInBuffer[this.NumberOfBuffers];
            for (int index = 0; index < this.buffers.Length; ++index)
                this.buffers[index] = new WaveInBuffer(this.waveInHandle, bufferSize);
        }

        private void OpenWaveInDevice()
        {
            this.CloseWaveInDevice();
            MmException.Try(
                WaveInterop.waveInOpenWindow(out this.waveInHandle, (IntPtr) this.DeviceNumber, this.WaveFormat,
                    this.callbackEvent.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero,
                    WaveInterop.WaveInOutOpenFlags.CallbackEvent), "waveInOpen");
            this.CreateBuffers();
        }

        public void StartRecording()
        {
            if (this.captureState != CaptureState.Stopped)
                return;
            this.OpenWaveInDevice();
            MmException.Try(WaveInterop.waveInStart(this.waveInHandle), "waveInStart");
            this.captureState = CaptureState.Starting;
            ThreadPool.QueueUserWorkItem((WaitCallback) (state => this.RecordThread()), (object) null);
        }

        private void RecordThread()
        {
            Exception e = (Exception) null;
            try
            {
                this.DoRecording();
            }
            catch (Exception ex)
            {
                e = ex;
            }
            finally
            {
                this.captureState = CaptureState.Stopped;
                this.RaiseRecordingStoppedEvent(e);
            }
        }

        private void DoRecording()
        {
            this.captureState = CaptureState.Capturing;
            foreach (WaveInBuffer buffer in this.buffers)
            {
                if (!buffer.InQueue)
                    buffer.Reuse();
            }

            while (this.captureState == CaptureState.Capturing)
            {
                if (this.callbackEvent.WaitOne())
                {
                    foreach (WaveInBuffer buffer in this.buffers)
                    {
                        if (buffer.Done)
                        {
                            if (buffer.BytesRecorded > 0)
                            {
                                EventHandler<WaveInEventArgs> dataAvailable = this.DataAvailable;
                                if (dataAvailable != null)
                                    dataAvailable((object) this,
                                        new WaveInEventArgs(buffer.Data, buffer.BytesRecorded));
                            }

                            if (this.captureState == CaptureState.Capturing)
                                buffer.Reuse();
                        }
                    }
                }
            }
        }

        private void RaiseRecordingStoppedEvent(Exception e)
        {
            EventHandler<StoppedEventArgs> handler = this.RecordingStopped;
            if (handler == null)
                return;
            if (this.syncContext == null)
                handler((object) this, new StoppedEventArgs(e));
            else
                this.syncContext.Post((SendOrPostCallback) (state => handler((object) this, new StoppedEventArgs(e))),
                    (object) null);
        }

        public void StopRecording()
        {
            if (this.captureState == CaptureState.Stopped)
                return;
            this.captureState = CaptureState.Stopping;
            MmException.Try(WaveInterop.waveInStop(this.waveInHandle), "waveInStop");
            MmException.Try(WaveInterop.waveInReset(this.waveInHandle), "waveInReset");
            this.callbackEvent.Set();
        }

        public long GetPosition()
        {
            MmTime mmTime = new MmTime();
            mmTime.wType = 4U;
            MmException.Try(
                WaveInterop.waveInGetPosition(this.waveInHandle, out mmTime, Marshal.SizeOf<MmTime>(mmTime)),
                "waveInGetPosition");
            return mmTime.wType == 4U
                ? (long) mmTime.cb
                : throw new Exception(string.Format("waveInGetPosition: wType -> Expected {0}, Received {1}",
                    (object) 4, (object) mmTime.wType));
        }

        public WaveFormat WaveFormat { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (this.captureState != CaptureState.Stopped)
                this.StopRecording();
            this.CloseWaveInDevice();
        }

        private void CloseWaveInDevice()
        {
            int num1 = (int) WaveInterop.waveInReset(this.waveInHandle);
            if (this.buffers != null)
            {
                for (int index = 0; index < this.buffers.Length; ++index)
                    this.buffers[index].Dispose();
                this.buffers = (WaveInBuffer[]) null;
            }

            int num2 = (int) WaveInterop.waveInClose(this.waveInHandle);
            this.waveInHandle = IntPtr.Zero;
        }

        public MixerLine GetMixerLine() => !(this.waveInHandle != IntPtr.Zero)
            ? new MixerLine((IntPtr) this.DeviceNumber, 0, MixerFlags.WaveIn)
            : new MixerLine(this.waveInHandle, 0, MixerFlags.WaveInHandle);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object) this);
        }
    }
}