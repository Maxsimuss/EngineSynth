using EngineSynth.Common.Util;

namespace EngineSynth.V2.Model
{
    class Harmonic
    {
        public float Ratio = 1;
        public readonly Automation Gain = new Automation();
        public readonly Automation Q = new Automation();
    }
}
