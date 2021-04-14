using CaptureMod.Connection;
using CaptureMod.Utils;
using NAudio.Wave;

namespace CaptureMod.Voice
{ 

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public static class VoIP
    {
        private static bool isActivated;
        
        private static MyWaveInEvent recorder;
        
        public static BufferedWaveProvider sender;

        public static MyBufferedSampleProvider receiver;
        
        private static MyFilter equalizer;

        private static float[] outBuffer;

        private static WaveOutEvent player;

        static VoIP()
        {
            recorder = new MyWaveInEvent {WaveFormat = new WaveFormat(44100, 16, 2), DeviceNumber = 0};
            recorder.DataAvailable += RecorderOnDataAvailable;
            sender = new BufferedWaveProvider(recorder.WaveFormat);
            sender.DiscardOnBufferOverflow = true;
            equalizer = new MyFilter(sender.ToSampleProvider(), 60, 1200, -2.5f, -4f);
            outBuffer = new float[sender.BufferedBytes];
            receiver = new MyBufferedSampleProvider(recorder.WaveFormat);
            //receiver.DiscardOnBufferOverflow = true;
            player = new WaveOutEvent();
            player.Init(receiver);
        }
        
        
        public static void Start()
        {
            if (isActivated) return;
            isActivated = true;
            "StartMic".Log();
            recorder.StartRecording();
            player.Play();
            "Play".Log();
        }

        public static void SetMic(bool status)
        {
            if (status)
            {
                recorder.StartRecording();
            }
            else
            {
                recorder.StopRecording();
            }
        }
        
        public static void Stop()
        {
            if (!isActivated) return;
            isActivated = false;
            "StopMic".Log();
            sender.ClearBuffer();
            receiver.ClearBuffer();
            recorder.StopRecording();
            player.Stop();
            "Stop".Log();
        }

        private static void RecorderOnDataAvailable(object _, WaveInEventArgs waveInEventArgs)
        {
            sender.AddSamples(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
            if (outBuffer == null || outBuffer.Length < waveInEventArgs.BytesRecorded / 2)
            {
                outBuffer = new float[waveInEventArgs.BytesRecorded / 2];
            }
            equalizer.Read(outBuffer, 0, sender.BufferedBytes/2);
        }

        public static void ReceiveDataAvailable(VoipMessage voipMessage)
        {
            var audio = voipMessage.FloatAudio();
            receiver.AddSamples(audio.Item1, (int) audio.Item2, (int) audio.Item3, voipMessage.Listeners);
            
        }
    }
}
