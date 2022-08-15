using EngineSynth.Game;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.IO;

namespace EngineSynth.Audio.Synth
{
    internal class Synth
    {
        private int SampleRate;
        private string basePath;
        private Engine engine;
        private SynthSettings settings;
        private Random random;
        private int randomSeed = 0;
        private double LongestSampleMs = 0;

        public Synth(Engine engine, SynthSettings settings, string basePath)
        {
            this.engine = engine;
            this.settings = settings;
            this.basePath = basePath;
            random = new Random();
            randomSeed = random.Next();
        }

        public void Run(string[] files)
        {
            Sample[] samples = new Sample[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                samples[i] = new Sample(files[i]);
                SampleRate = samples[i].SampleRate;
            }


            SaveSound(engine.name + "_engine", samples, 500f, .25f);
            SaveSound(engine.name + "_exhaust", samples, SampleRate / 2f, 1f);
        }

        private Sample[] PreProcess(Sample[] samplesIn, float filterFreq, float gain)
        {
            Sample[] samples = new Sample[samplesIn.Length];
            for (int i = 0; i < samplesIn.Length; i++)
            {
                BiQuad f = new HighShelfFilter(SampleRate, engine.FilterFreq, -12);
                f.Q = 20;

                Sample sample = samplesIn[i].Clone().Stretch(engine.Pitch).LowPass(filterFreq, 5f, gain).Filter(f);
                LongestSampleMs = Math.Max(LongestSampleMs, SamplesToMs(sample.Length));

                samples[i] = sample;
            }

            return samples;
        }

        private void SaveSound(string name, Sample[] samplesIn, float filterFreq, float gain)
        {
            SfxBlendBuilder builder = new SfxBlendBuilder();
            
            var samples = PreProcess(samplesIn, Math.Min(filterFreq, engine.OffLoadFilter), gain);
            SaveSounds(engine, name, samples, SoundType.OffLoad, builder);

            samples = PreProcess(samplesIn, filterFreq, gain);
            SaveSounds(engine, name, samples, SoundType.OnLoad, builder);

            string jsonPath = basePath + "/art/sound/blends/";
            if (!Directory.Exists(jsonPath))
                Directory.CreateDirectory(jsonPath);
            File.WriteAllText(jsonPath + name + ".sfxBlend2D.json", builder.Build());
        }

        private void SaveSounds(Engine engine, string name, Sample[] samples, SoundType type, SfxBlendBuilder builder)
        {
            random = new Random(randomSeed);

            string artDir = "/art/sound/engine/" + name + "/";
            float gain = type == SoundType.OnLoad ? engine.OnGain : engine.OffGain;
            for (int rpm = engine.MinRPM; rpm <= engine.MaxRPM; rpm += engine.RPMInc)
            {
                string fileName = rpm + type.Suffix + ".wav";
                string wavPath = basePath + artDir + fileName;
                if (!Directory.Exists(basePath + artDir))
                    Directory.CreateDirectory(basePath + artDir);
                using(WaveWriter waveWriter = new WaveWriter(wavPath, new WaveFormat(SampleRate, 32, 1, AudioEncoding.IeeeFloat)))
                {
                    float[] res = GenerateSound(rpm, samples, gain + (float)Math.Pow(rpm / 6000f, 2) * .2f);

                    waveWriter.WriteSamples(res, 0, res.Length);
                }

                builder.Add(type, artDir + fileName, rpm);
            }
        }

        private unsafe float[] GenerateSound(int rpm, Sample[] samples, float gain)
        {
            double msPerRotation = 1000 / (rpm / 60D);
            double msPerPS = msPerRotation / engine.PowerstrokesPerRotation;
            double msLength = LongestSampleMs * 2 + Math.Max((msPerPS + engine.Randomness * msPerPS / 5) * (engine.CylinderCount * 32), 4000);
            double nextSampleMs = 0;
            int cylinderCounter = 0;
            
            Sample result = new Sample((int)MsToSamples(msLength), SampleRate);

            List<int> positions = new List<int>();
            while (!result.Add((int)MsToSamples(nextSampleMs), samples[engine.FiringOrder[cylinderCounter % engine.CylinderCount] - 1].Buffer))
            {
                if (nextSampleMs > LongestSampleMs)
                {
                    positions.Add((int)MsToSamples(nextSampleMs));
                }

                cylinderCounter++;
                nextSampleMs += msPerPS + (random.NextDouble()) * engine.Randomness * msPerPS / 5;
            }

            int last = (positions.Count - 1) / engine.CylinderCount * engine.CylinderCount;

            result.LoopStart = positions[0];
            result.LoopEnd = positions[last];
            float baseFreq = rpm * engine.PowerstrokesPerRotation / 60;

            return PostProcess(result, baseFreq, gain + rpm / 1000 * engine.RpmGain);
        }

        private float[] PostProcess(Sample input, float baseFreq, float gain)
        {
            List<BiQuad> filters = new List<BiQuad>();
            filters.Add(new HighpassFilter(SampleRate, 20));
            filters.Add(new LowpassFilter(SampleRate, 20000));
            filters.Add(new PeakFilter(SampleRate, baseFreq / 2, 5, engine.BaseFreqGain / 4));
            filters.Add(new PeakFilter(SampleRate, baseFreq, 5, engine.BaseFreqGain));
            filters.Add(new PeakFilter(SampleRate, baseFreq * 1.5, 5, engine.BaseFreqGain / 2));
            filters.Add(new PeakFilter(SampleRate, baseFreq * 2, 5, engine.BaseFreqGain / 2));
            filters.Add(new PeakFilter(SampleRate, baseFreq * 2.5, 5, engine.BaseFreqGain / 4));
            filters.Add(new PeakFilter(SampleRate, baseFreq * 3, 5, engine.BaseFreqGain / 4));

            for (int i = 0; i < settings.filters.Count; i++)
            {
                FilterData d = settings.filters[i];
                filters.Add(new PeakFilter(SampleRate, d.Frequency, d.Q, d.Gain));
            }

            for (int j = 0; j < filters.Count; j++)
            {
                input.Filter(filters[j]);
            }

            input.Loop((int)MsToSamples(1));

            for (int i = 0; i < input.Length; i++)
            {
                float val = input.Buffer[i] * gain;
                float aVal = Math.Abs(val);
                input.Buffer[i] = aVal / (aVal + 1) * Math.Sign(val);
            }

            return input.Buffer;
        }

        private double MsToSamples(double ms)
        {
            return ms / 1000D * SampleRate;
        }

        private double SamplesToMs(double samples)
        {
            return samples * 1000D / SampleRate;
        }
    }
}
