using CSCore.DSP;
using EngineSynth.Common.Audio;
using EngineSynth.Common.Util;
using System;
using System.Collections.Generic;

namespace EngineSynth.V2.Model
{
    internal class Synth : AbstractSynth
    {
        private static readonly int INTERNAL_SAMPLERATE = 44100;

        public List<string> SamplePaths;
        public List<Automation> Automations = new List<Automation>();
        public Automation gain = new Automation() { ScaleX = 20000, ScaleY = 8, Name = "Gain" };
        public Automation randomness = new Automation() { ScaleX = 20000, ScaleY = 1, Name = "Randomness" };
        public Automation samplePitch = new Automation() { ScaleX = 20000, ScaleY = 2, Name = "Pitch" };
        public Automation sampleLowpass = new Automation() { ScaleX = 20000, ScaleY = 20000, Name = "Lowpass" };
        public Automation sampleHighpass = new Automation() { ScaleX = 20000, ScaleY = 20000, Name = "Highpass" };
        public Automation HarmonicGain = new Automation() { ScaleX = 20000, ScaleY = 6, Name = "Harmonic Gain" };
        public List<Harmonic> Harmonics = new List<Harmonic>();

        private Sample[] samples = null;

        public Synth()
        {
            SamplePaths = new List<string>();

            Automations.Add(gain);
            Automations.Add(randomness);
            Automations.Add(samplePitch);
            Automations.Add(sampleLowpass);
            Automations.Add(sampleHighpass);
            Automations.Add(HarmonicGain);

            gain.AddPoint(0, .5f);
            gain.AddPoint(500, .5f);
            gain.AddPoint(1250, 1);
            gain.AddPoint(6000, 1);
            gain.AddPoint(8000, 3);
            gain.AddPoint(9000, 2);
            gain.AddPoint(20000, 1);

            randomness.AddPoint(1000, .5f);
            randomness.AddPoint(2000, .2f);
            randomness.AddPoint(8000, .05f);
            randomness.AddPoint(9000, .05f);
            randomness.AddPoint(20000, .2f);

            samplePitch.AddPoint(0, 1f);
            samplePitch.AddPoint(2000, 1);
            samplePitch.AddPoint(5800, 1);
            samplePitch.AddPoint(6200, 1.1f);
            samplePitch.AddPoint(9000, 1f);
            samplePitch.AddPoint(20000, .8f);

            sampleLowpass.AddPoint(0, 1000);
            sampleLowpass.AddPoint(1000, 3000);
            sampleLowpass.AddPoint(7000, 4000);
            sampleLowpass.AddPoint(9000, 10000);
            sampleLowpass.AddPoint(20000, 8000);

            sampleHighpass.AddPoint(0, 20);
            sampleHighpass.AddPoint(9000, 20);
            sampleHighpass.AddPoint(20000, 1000);
        }

        public int randomSeed = 0;
        private double MsToSamples(double ms)
        {
            return ms / 1000D * INTERNAL_SAMPLERATE;
        }

        private double SamplesToMs(double samples)
        {
            return samples * 1000D / INTERNAL_SAMPLERATE;
        }

        public void Init()
        {
            randomSeed = new Random().Next();
            samples = new Sample[CylinderCount];

            samples = new Sample[SamplePaths.Count];
            for (int i = 0; i < SamplePaths.Count; i++)
            {
                samples[i] = new Sample(SamplePaths[i]).RemoveSilence();
            }
        }

        public override Sample GenerateLoopForRpm(int rpm, int loopLength = 32, int minTime = 4000, bool deterministic = true)
        {
            if (rpm < 1)
            {
                throw new InvalidOperationException("rpm may not be lower than 1!");
            }

            // Initialize random number generator
            Random random;
            if (deterministic)
            {
                random = new Random(randomSeed);
            }
            else
            {
                random = new Random();
            }

            for (int i = 0; i < SamplePaths.Count; i++)
            {
                samples[i] = new Sample(SamplePaths[i]).RemoveSilence();
            }

            // Calculate the longest processed sample length
            int longestProcessedSample = 0;
            Sample[] processedSamples = new Sample[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                Sample processedSample = samples[i].Clone()
                    .Stretch(samplePitch.GetValueAt(rpm))
                    .Filter(new LowpassFilter(INTERNAL_SAMPLERATE, Math.Clamp(sampleLowpass.GetValueAt(rpm), 10, 20000)))
                    .Filter(new HighpassFilter(INTERNAL_SAMPLERATE, Math.Clamp(sampleHighpass.GetValueAt(rpm), 10, 20000)))
                    .RemoveSilence();
                processedSamples[i] = processedSample;

                longestProcessedSample = Math.Max(processedSample.Length, longestProcessedSample);
            }

            // Calculate time parameters
            double msPerRotation = 1000 / (rpm / 60D);
            double msPerPowerStroke = msPerRotation / PowerstrokesPerRevolution;

            // Calculate total length in samples
            float randomnessFactor = randomness.GetValueAt(rpm);
            int totalSamplesRequired = (int)MsToSamples(Math.Max(CylinderCount * loopLength * (msPerPowerStroke + msPerPowerStroke * randomnessFactor * .5), minTime));

            // Initialize the result sample
            Sample result = new Sample(totalSamplesRequired + longestProcessedSample * 2, INTERNAL_SAMPLERATE);

            int cylinderIndex = 0;
            double currentSampleOffset = 0;

            // Generate loop samples

            //while (true)
            //{
            //    result.Add((int)currentSampleOffset, processedSamples[(cylinderIndex++ % CylinderCount)].Buffer);

            //    if (currentSampleOffset >= totalSamplesRequired && cylinderIndex % CylinderCount == 0)
            //    {
            //        break;
            //    }

            //    currentSampleOffset += MsToSamples(msPerPowerStroke + (random.NextDouble() - 0.5) * msPerPowerStroke * randomnessFactor);
            //}
            while (currentSampleOffset < totalSamplesRequired || cylinderIndex % CylinderCount != 0)
            {
                result.Add((int)currentSampleOffset, processedSamples[cylinderIndex++ % CylinderCount].Buffer);

                currentSampleOffset += MsToSamples(msPerPowerStroke + (random.NextDouble() - 0.5) * msPerPowerStroke * randomnessFactor);
            }

            // Add tail to the result
            float[] tail = new float[Math.Min((int)currentSampleOffset, result.Buffer.Length - (int)currentSampleOffset)];
            Array.Copy(result.Buffer, (int)currentSampleOffset, tail, 0, tail.Length);
            result.Add(0, tail);

            // Set loop start and end
            result.LoopStart = 0;
            result.LoopEnd = (int)currentSampleOffset;

            // Apply post-processing and return
            return PostProcess(result, rpm);
        }

        private Sample PostProcess(Sample input, int rpm)
        {
            List<BiQuad> filters = new List<BiQuad>
            {
                new HighpassFilter(INTERNAL_SAMPLERATE, 20),
                new LowpassFilter(INTERNAL_SAMPLERATE, 20000)
            };

            float baseFreq = rpm * PowerstrokesPerRevolution / 60F;
            foreach (Harmonic harmonic in Harmonics)
            {
                float harmonicRatio = baseFreq * harmonic.Ratio;
                float harmonicQ = harmonic.Q.GetValueAt(rpm);
                float harmonicGain = HarmonicGain.GetValueAt(rpm) * harmonic.Gain.GetValueAt(rpm);

                new PeakFilter(INTERNAL_SAMPLERATE, harmonicRatio, harmonicQ, harmonicGain).Process(input.Buffer);
            }

            float globalGain = gain.GetValueAt(rpm);
            for (int i = 0; i < input.Length; i++)
            {
                float value = input.Buffer[i] * globalGain;
                float absoluteValue = Math.Abs(value);
                input.Buffer[i] = absoluteValue / (absoluteValue + 1) * Math.Sign(value);
            }

            foreach (var filter in filters)
            {
                input.Filter(filter);
            }

            input.Loop();

            return input;
        }

        public byte[] GenerateBuffer(float RPM, float prevRPM, bool load, int length)
        {
            Sample result0 = GenerateLoopForRpm((int)prevRPM, 1, length, true);
            Sample result1 = GenerateLoopForRpm((int)RPM, 1, length, true);

            Sample result = new Sample(Math.Max(result0.Length, result1.Length), INTERNAL_SAMPLERATE);
            int i = 0;
            float pos0 = 0, pos1 = 0;

            while (true)
            {
                double t = Math.Min(i / Math.Min((double)result0.Length / prevRPM * RPM, (double)result1.Length * prevRPM / RPM), 1);
                double interpolatedRPM = RPM * t + prevRPM * (1 - t);

                double rate0 = interpolatedRPM / prevRPM;
                double rate1 = interpolatedRPM / RPM;

                pos0 += (float)rate0;
                pos1 += (float)rate1;

                if (pos0 >= result0.Length || pos1 >= result1.Length)
                {
                    break;
                }

                result[i] = (float)(result1.SampleHermite(pos1) * Math.Sqrt(t) + result0.SampleHermite(pos0) * Math.Sqrt(1 - t));

                if (float.IsNaN(result[i]))
                {
                    throw new Exception();
                }
                i++;
            }

            int size = i * 4;

            byte[] output = new byte[size];
            Buffer.BlockCopy(result.Buffer, 0, output, 0, size);

            return output;
        }
    }
}
