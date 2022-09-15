using CSCore;
using CSCore.SoundOut;
using CSCore.Streams;
using System;
using System.Threading.Tasks;

namespace EngineSynth.Model.Audio
{
    internal class AudioStreamPlayer
    {
        private WriteableBufferingSource source;

        WasapiOut soundOut;
        public AudioStreamPlayer(int SampleRate)
        {
            source = new WriteableBufferingSource(new WaveFormat(SampleRate, 32, 1, AudioEncoding.IeeeFloat), 20000000);
            soundOut = new WasapiOut();
            soundOut.Initialize(source);
            soundOut.Stopped += SoundOut_Stopped;
        }

        public void PlayAsync()
        {
            Task.Run(soundOut.Play);
        }

        public void Buffer(float[] samples)
        {
            byte[] data = new byte[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(samples[i]), 0, data, i * 4, 4);
            }
            source.Write(data, 0, data.Length);
        }

        private void SoundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            ((WasapiOut)sender).Dispose();
        }
    }
}
