using System;

namespace EngineSynth.Model
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
