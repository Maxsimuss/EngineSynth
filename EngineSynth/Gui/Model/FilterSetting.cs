using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Gui.Model
{
    public class FilterSetting
    {
        private float frequency = 20;
        public float Frequency 
        {
            get { return frequency; }
            set 
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Frequency");

                frequency = value; 
            }
        }


        private float q = .71f;
        public float Q 
        {
            get { return q; }
            set
            {
                if(value <= 0)
                    throw new ArgumentOutOfRangeException("Q");
                
                q = value;
            }
        }

        public float Gain { get; set; } = 0;
    }
}
