using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Audio
{
    internal class Engine
    {
        public string name = "I6_2";
        public int CylinderCount = 6;
        public int PowerstrokesPerRotation = 3;
        public int[] FiringOrder = new int[] { 1, 5, 3, 6, 2, 4 };
        public int MinRPM = 100;
        public int MaxRPM = 10000;
        public int RPMInc = 100;
        public float OnGain = 1;
        public float OffGain = 1;
        public float Randomness = .2f;
        public float Pitch = 1;
        public float BaseFreqGain = 0;
        public float FilterFreq = 2000;
        public float RpmGain = .5f;
        public float OffLoadFilter = 500f;
    }
}
