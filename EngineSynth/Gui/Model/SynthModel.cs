﻿using CSCore;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using EngineSynth.Audio;
using EngineSynth.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EngineSynth.Gui.Model
{
    public class SynthModel
    {
        public string ExportPath { get; set; } = string.Empty;
        public string Name { get; set; } = "I6_2";
        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnStateChanged?.Invoke();
            }
        }

        public event OnStateChangedEvent OnStateChanged;
        public delegate void OnStateChangedEvent();

        public List<string> Samples;
        public List<FilterSetting> Filters;
        public List<ResonanceSetting> Resonances;
        public List<Setting> Settings;
        private readonly Setting CylinderCount, PowerPerRev, Randomness, FilterFreq, OffLoadFilter, Pitch, MinRPM, MaxRPM, RPMInc, OnGain, OffGain, RpmGain, BaseFreqGain;

        public SynthModel()
        {
            Filters = new List<FilterSetting>();
            Resonances = new List<ResonanceSetting>();
            Samples = new List<string>();
            Settings = new List<Setting>();

            CylinderCount = AddSetting("Cylinder Count", 6, 1, 16, 1);
            PowerPerRev = AddSetting("Powerstrokes Per Revolution", 3, 1, 16, 1);
            Randomness = AddSetting("Randomness", .2, 0, 1, .01);
            FilterFreq = AddSetting("Sample Low Pass", 3000, 20, 20000, 1);
            OffLoadFilter = AddSetting("OffLoad Low Pass", 1500, 20, 20000, 1);
            Pitch = AddSetting("Sample Pitch", 1, .2, 2, .01);
            MinRPM = AddSetting("Min RPM", 500, 100, 2000, 100);
            MaxRPM = AddSetting("Max RPM", 9000, 200, 20000, 100);
            RPMInc = AddSetting("RPM Increment", 500, 100, 2000, 100);
            OnGain = AddSetting("On Load Gain", 1, 0, 5, .01);
            OffGain = AddSetting("Off Load Gain", .7, 0, 5, .01);
            RpmGain = AddSetting("Rpm Gain", .25, 0, 2, .01);
            BaseFreqGain = AddSetting("Base Frequency Gain", 0, -30, 30, .01);

        }

        private Setting AddSetting(string name, double value, double min, double max, double inc)
        {
            Setting setting = new Setting(name, value, min, max, inc);
            Settings.Add(setting);

            return setting;
        }
        
        
        //bad
        public void AddFilter()
        {
            Filters.Add(new FilterSetting());

            OnStateChanged?.Invoke();
        }

        public void RemoveFilter(FilterSetting filterSetting)
        {
            Filters.Remove(filterSetting);

            OnStateChanged?.Invoke();
        }

        public void AddResonance()
        {
            Resonances.Add(new ResonanceSetting());

            OnStateChanged?.Invoke();
        }

        public void RemoveResonance(ResonanceSetting res)
        {
            Resonances.Remove(res);

            OnStateChanged?.Invoke();
        }
        //


        public void Render()
        {
            if (ExportPath == string.Empty)
            {
                throw new Exception("No export path specified!");
            }

            IsBusy = true;
            Task.Run(() =>
            {
                random = new Random();
                randomSeed = random.Next();

                Sample[] samples = new Sample[Samples.Count];
                for (int i = 0; i < Samples.Count; i++)
                {
                    samples[i] = new Sample(Samples[i]);
                    SampleRate = samples[i].SampleRate;
                }


                SaveSound(Name + "_engine", samples, 500f, .25f);
                SaveSound(Name + "_exhaust", samples, SampleRate / 2f, 1f);

                IsBusy = false;
            });
        }

        private int SampleRate;
        private Random random;
        private int randomSeed = 0;
        private double LongestSampleMs = 0;

        private Sample[] PreProcess(Sample[] samplesIn, float filterFreq, float gain)
        {
            Sample[] samples = new Sample[samplesIn.Length];
            for (int i = 0; i < samplesIn.Length; i++)
            {
                BiQuad f = new HighShelfFilter(SampleRate, FilterFreq, -12);
                f.Q = 20;

                Sample sample = samplesIn[i].Clone().Stretch(Pitch).LowPass(filterFreq, 5f, gain).Filter(f);
                LongestSampleMs = Math.Max(LongestSampleMs, SamplesToMs(sample.Length));

                samples[i] = sample;
            }

            return samples;
        }

        private void SaveSound(string name, Sample[] samplesIn, float filterFreq, float gain)
        {
            SfxBlendBuilder builder = new SfxBlendBuilder();

            var samples = PreProcess(samplesIn, Math.Min(filterFreq, OffLoadFilter), gain);
            SaveSounds(name, samples, SoundType.OffLoad, builder);

            samples = PreProcess(samplesIn, filterFreq, gain);
            SaveSounds(name, samples, SoundType.OnLoad, builder);

            string jsonPath = ExportPath + "/art/sound/blends/";
            if (!Directory.Exists(jsonPath))
                Directory.CreateDirectory(jsonPath);
            File.WriteAllText(jsonPath + name + ".sfxBlend2D.json", builder.Build());
        }

        private void SaveSounds(string name, Sample[] samples, SoundType type, SfxBlendBuilder builder)
        {
            random = new Random(randomSeed);

            string artDir = "/art/sound/engine/" + name + "/";
            float gain = type == SoundType.OnLoad ? OnGain : OffGain;
            for (int rpm = MinRPM; rpm <= MaxRPM; rpm += RPMInc)
            {
                string fileName = rpm + type.Suffix + ".wav";
                string wavPath = ExportPath + artDir + fileName;
                if (!Directory.Exists(ExportPath + artDir))
                    Directory.CreateDirectory(ExportPath + artDir);
                using (WaveWriter waveWriter = new WaveWriter(wavPath, new WaveFormat(SampleRate, 32, 1, AudioEncoding.IeeeFloat)))
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
            double msPerPS = msPerRotation / PowerPerRev;
            double msLength = LongestSampleMs * 2 + Math.Max((msPerPS + Randomness * msPerPS / 5) * (CylinderCount * 32), 4000);
            double nextSampleMs = 0;
            int cylinderCounter = 0;

            Sample result = new Sample((int)MsToSamples(msLength), SampleRate);

            List<int> positions = new List<int>();
            while (!result.Add((int)MsToSamples(nextSampleMs), samples[cylinderCounter % CylinderCount].Buffer))
            {
                if (nextSampleMs > LongestSampleMs)
                {
                    positions.Add((int)MsToSamples(nextSampleMs));
                }

                cylinderCounter++;
                nextSampleMs += msPerPS + (random.NextDouble()) * Randomness * msPerPS / 5;
            }

            int last = (positions.Count - 1) / (int)CylinderCount * CylinderCount;

            result.LoopStart = positions[0];
            result.LoopEnd = positions[last];
            float baseFreq = rpm * PowerPerRev / 60F;

            return PostProcess(result, baseFreq, gain + rpm / 1000f * RpmGain);
        }

        private float[] PostProcess(Sample input, float baseFreq, float gain)
        {
            List<BiQuad> filters = new List<BiQuad>
            {
                new HighpassFilter(SampleRate, 20),
                new LowpassFilter(SampleRate, 20000),


                //2004 f1 v10
                //new PeakFilter(SampleRate, baseFreq / 5, 5, BaseFreqGain / 4),
                //new PeakFilter(SampleRate, baseFreq / 3.5, 5, BaseFreqGain / 2),
                //new PeakFilter(SampleRate, baseFreq / 2.5, 5, BaseFreqGain / 4),
                //new PeakFilter(SampleRate, baseFreq / 2, 5, BaseFreqGain),
                //new PeakFilter(SampleRate, baseFreq / 2 * 1.2, 5, BaseFreqGain / 4),
                //new PeakFilter(SampleRate, baseFreq / 2 * 1.4, 5, BaseFreqGain / 4),
                //new PeakFilter(SampleRate, baseFreq / 2 * 1.6, 5, BaseFreqGain / 4),
                //new PeakFilter(SampleRate, baseFreq / 2 * 1.8, 5, BaseFreqGain / 4),
                //new PeakFilter(SampleRate, baseFreq, 5, BaseFreqGain / 2),
                //new PeakFilter(SampleRate, baseFreq * 2, 5, BaseFreqGain / 4)

                //2014 - ??? f1 v6
                //new PeakFilter(SampleRate, baseFreq, 5, BaseFreqGain),
                //new PeakFilter(SampleRate, baseFreq / 1.25, 5, BaseFreqGain / 2),
                //new PeakFilter(SampleRate, baseFreq / 1.5, 5, BaseFreqGain),
                //new PeakFilter(SampleRate, baseFreq / 2, 5, BaseFreqGain)
            };

            foreach (ResonanceSetting f in Resonances)
            {
                filters.Add(new PeakFilter(SampleRate, baseFreq * f.Multiplier, 5, BaseFreqGain * f.Gain));
            }

            for (int i = 0; i < Filters.Count; i++)
            {
                FilterSetting f = Filters[i];
                filters.Add(new PeakFilter(SampleRate, f.Frequency, 1/f.Q, f.Gain));
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

        public void LoadConfig()
        {
            if (ExportPath == string.Empty)
            {
                throw new Exception("No export path specified!");
            }

            Config.Config cfg = new Config.Config();
            if (!cfg.Load(ExportPath + "/" + Name + ".ecfg")) return;

            foreach (var item in cfg.Settings)
            {
                Setting setting = Settings.Find(s => s.Name == item.Key);

                if (setting != null)
                {
                    setting.Value = item.Value;
                }
                else
                {
                    Debug.Print("Setting not found! Expected: {0}", item.Key);
                }
            }

            Samples = cfg.Samples;
            Filters = cfg.Filters;
            Resonances = cfg.Resonances;

            OnStateChanged();
        }

        public void SaveConfig()
        {
            if (ExportPath == string.Empty)
            {
                throw new Exception("No export path specified!");
            }

            Config.Config cfg = new Config.Config();

            foreach (Setting item in Settings)
            {
                cfg.Settings.Add(item.Name, item.Value);
            }
            cfg.Samples.AddRange(Samples);
            cfg.Filters.AddRange(Filters);
            cfg.Resonances.AddRange(Resonances);

            cfg.Save(ExportPath + "/" + Name + ".ecfg");
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
