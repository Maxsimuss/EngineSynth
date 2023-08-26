using EngineSynth.Common.Util;
using System.Collections.Generic;

namespace EngineSynth.V2.Model
{
    class SynthConfig
    {
        public List<string> SamplePaths;
        public Automation gain = new Automation();
        public Automation randomness = new Automation();
        public Automation samplePitch = new Automation();
        public Automation sampleLowpass = new Automation();
        public Automation sampleHighpass = new Automation();
        public Automation HarmonicGain = new Automation();
        public List<Harmonic> Harmonics = new List<Harmonic>();

        public SynthConfig()
        {

        }

        public SynthConfig(Synth synth)
        {
            SamplePaths = synth.SamplePaths;
            gain = synth.gain;
            randomness = synth.randomness;
            samplePitch = synth.samplePitch;
            sampleLowpass = synth.sampleLowpass;
            sampleHighpass = synth.sampleHighpass;
            HarmonicGain = synth.HarmonicGain;
            Harmonics = synth.Harmonics;
        }

        public void ApplyTo(Synth synth)
        {
            synth.SamplePaths = SamplePaths;
            synth.gain = gain;
            synth.randomness = randomness;
            synth.samplePitch = samplePitch;
            synth.sampleLowpass = sampleLowpass;
            synth.sampleHighpass = sampleHighpass;
            synth.HarmonicGain = HarmonicGain;
            synth.Harmonics = Harmonics;

            synth.Automations = new List<Automation>
            {
                gain,
                randomness,
                samplePitch,
                sampleLowpass,
                sampleHighpass,
                HarmonicGain
            };
        }
    }
}
