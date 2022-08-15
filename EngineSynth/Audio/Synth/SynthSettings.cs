using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Audio.Synth
{
    internal class SynthSettings
    {
        public List<FilterData> filters;

        public SynthSettings()
        {
            filters = new List<FilterData>();
        }
    }
}
