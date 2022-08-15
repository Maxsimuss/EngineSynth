using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Audio.Synth
{
    internal class FilterData
    {
        public double Frequency, Q, Gain;

        public FilterData(double frequency, double q, double gain)
        {
            Frequency = frequency;
            Q = q;
            Gain = gain;
        }
    }
}
