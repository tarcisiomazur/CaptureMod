using System;
using CaptureMod.Interface;
using NAudio.Wave;

namespace CaptureMod.Voice
{
    public class MyBufferedSampleProvider : IWaveProvider
    {
        private MyCircularBuffer circularBuffer;
        private readonly WaveFormat waveFormat;

        public MyBufferedSampleProvider(WaveFormat waveFormat)
        {
            this.waveFormat = waveFormat;
            this.BufferLength = (waveFormat.AverageBytesPerSecond / 4) * 5;
            this.ReadFully = true;
            FloatPerRead = waveFormat.AverageBytesPerSecond / 20;
            this.circularBuffer = new MyCircularBuffer(this.BufferLength, FloatPerRead);
        }

        public bool ReadFully { get; set; }

        public int BufferLength { get; set; }

        public bool DiscardOnBufferOverflow { get; set; }

        public int Read(byte[] buffer, int offset, int count)
        {
            var num = 0;
            if (this.circularBuffer != null)
            {
                num = this.circularBuffer.Read(buffer, offset, count, out var listeners);
                UIVoIP.SetListeners(listeners);
            }

            if (this.ReadFully && num < count)
            {
                Array.Clear(buffer, offset + num, count - num);
                num = count;
            }

            return num;
        }

        public WaveFormat WaveFormat => this.waveFormat;

        public void AddSamples(float[] buffer, int offset, int count, byte[] listeners)
        {
            if (this.circularBuffer == null)
                this.circularBuffer = new MyCircularBuffer(this.BufferLength, FloatPerRead);
            circularBuffer.Write(buffer, offset, count, listeners);
      
        }

        public int ToRead => circularBuffer.Count / FloatPerRead;
    
        public int FloatPerRead { get; set; }

        public int Read(float[] buffer, int offset, int count)
        {
            int num = 0;
            if (this.circularBuffer != null)
            {
                num = this.circularBuffer.Read(buffer, offset, count);
                //Console.WriteLine($"{num} readed to buffer = {circularBuffer.Count}");
            }
            if (this.ReadFully && num < count)
            {
                Array.Clear(buffer, offset + num, count - num);
                num = count;
            }

            return num;
        }

        public void ClearBuffer()
        {
            circularBuffer?.Reset();
        }
    }
}