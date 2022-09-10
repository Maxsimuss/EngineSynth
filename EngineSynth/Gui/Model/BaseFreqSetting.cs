using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Gui.Model
{
    public class ResonanceSetting
    {
        private float multiplier = 1;
        public float Multiplier
        {
            get { return multiplier; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Multiplier");

                multiplier = value;
            }
        }

        public float Gain { get; set; } = 0;
    }
}
