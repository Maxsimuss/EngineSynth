using System;

namespace EngineSynth.v1.Model
{
    public class Setting
    {
        public string Name { get; set; }

        private double value = .5;
        public double Value
        {
            get { return value; }
            set
            {
                double v = Math.Clamp((int)(value / Increment) * Increment, Min, Max);

                if (v != this.value)
                {
                    this.value = v;
                }
            }
        }
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 1;
        public double Increment { get; set; } = 1;

        public static implicit operator double(Setting s) => s.Value;
        public static implicit operator float(Setting s) => (float)s.Value;
        public static implicit operator int(Setting s) => (int)s.Value;

        public Setting(string name, double value, double min, double max, double increment)
        {
            Name = name;
            Min = min;
            Max = max;
            Increment = increment;
            Value = value;
        }
    }
}
