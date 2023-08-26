using CSCore;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;

namespace EngineSynth.V2.Model
{
    internal class EngineSampleSource : IWaveSource
    {
        private Synth offLoadSynth, onLoadSynth;
        private Queue<byte> data = new Queue<byte>();

        private float prevRPM = 1000;
        public float RPM;
        public bool Load;

        public EngineSampleSource(Synth offLoadSynth, Synth onLoadSynth)
        {
            this.offLoadSynth = offLoadSynth;
            this.onLoadSynth = onLoadSynth;
        }

        public bool CanSeek => throw new NotImplementedException();

        public WaveFormat WaveFormat => new WaveFormat(44100, 32, 1, AudioEncoding.IeeeFloat);

        public long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public long Length => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            while (data.Count < count)
            {
                //int length = prevRPM == rpm ? 1000 : 100; // attempt at masking static sounds repeating
                int length = 100;
                byte[] _data = (Load ? onLoadSynth : offLoadSynth).GenerateBuffer(RPM, prevRPM, Load, length);
                prevRPM = RPM;

                for (int i = 0; i < _data.Length; i++)
                {
                    data.Enqueue(_data[i]);
                }
            }

            if (data.Count < count)
            {
                return count;
            }

            for (int i = 0; i < count; i++)
            {
                buffer[i] = data.Dequeue();
            }

            return count;
        }
    }

    internal class RealtimePlayer
    {
        private WasapiOut waveOut = new WasapiOut();
        private EngineSampleSource src;

        public RealtimePlayer(Synth offLoadSynth, Synth onLoadSynth)
        {
            src = new EngineSampleSource(offLoadSynth, onLoadSynth);

            waveOut.Stopped += (s, e) =>
            {
                if (e.HasError)
                {
                    throw e.Exception;
                }
            };
        }

        public float RPM { get => src.RPM; set => src.RPM = value; }
        public bool Load { get => src.Load; set => src.Load = value; }

        public void Start()
        {
            if (waveOut.PlaybackState != PlaybackState.Stopped) return;

            waveOut.Initialize(src);
            waveOut.Volume = .2f;
            waveOut.Play();
        }

        public void Stop()
        {
            if (waveOut.PlaybackState != PlaybackState.Playing) return;
            waveOut.Stop();
        }
    }
}
