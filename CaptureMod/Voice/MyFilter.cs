using System;
using CaptureMod.Connection;
using NAudio.Dsp;
using NAudio.Wave;

namespace CaptureMod.Voice
{
    public class MyFilter : ISampleProvider
    {
        private ISampleProvider sourceProvider;
        private float low, high;
        private float active, desactive;
        private int channels;
        private bool cut;
        private BiQuadFilter[] filtersLow;
        private BiQuadFilter[] filtersHigh;

        private float[] circ = new float[5000];
        private float[] circ2 = new float[500];
        private int icirc, icirc2;
        private float temp;
        private float sum, sum2;

        private float Next(float value)
        {
            temp = circ[icirc];
            circ[icirc] = value;
            icirc = ++icirc % 5000;
            return (float) ((value != 0 ? Math.Log(Math.Abs(value)) : 0) - (temp != 0 ? Math.Log(Math.Abs(temp)) : 0));
        }
        
        private float Next2(float value)
        {
            temp = circ2[icirc2];
            circ2[icirc2] = value;
            icirc2 = ++icirc2 % 500;
            return (float) ((value != 0 ? Math.Log(Math.Abs(value)) : 0) - (temp != 0 ? Math.Log(Math.Abs(temp)) : 0));
        }

        public MyFilter (ISampleProvider sourceProvider, int low, int high, float active, float desactive)
        {
            this.sourceProvider = sourceProvider;
            this.low = low;
            this.high = high;
            this.active = active;
            this.desactive = desactive;
            channels = sourceProvider.WaveFormat.Channels;
            filtersLow = new BiQuadFilter[channels];
            filtersHigh = new BiQuadFilter[channels];
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int n = 0; n < channels; n++)
            {
                if (filtersLow[n] == null)
                    filtersLow[n] = BiQuadFilter.LowPassFilter(44100, high, 1);
                else
                    filtersLow[n].SetLowPassFilter(44100, high, 1);
                
                if (filtersHigh[n] == null)
                    filtersHigh[n] = BiQuadFilter.HighPassFilter(44100, low, 1);
                else
                    filtersHigh[n].SetHighPassFilter(44100, low, 1);

            }
        }

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);
            float val;
            for (int i = 0, j = offset; i < samplesRead; i++, j++)
            {
                val = filtersLow[(i % channels)].Transform(buffer[j]);
                val = filtersHigh[(i % channels)].Transform(val);
                sum += Next(val);
                sum2 += Next2(val);
                if (sum / 5000 < desactive)
                    cut = true;
                else if (sum2 / 500 > active)
                    cut = false;
                if (cut)
                {
                    buffer[j] = 0;
                }
                else
                {
                    buffer[j] = val;
                }
            }
            VoIPConnection.Send(buffer, offset, samplesRead);
            return samplesRead;
        }
    }
}