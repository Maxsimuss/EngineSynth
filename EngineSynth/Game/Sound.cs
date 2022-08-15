using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Game
{
    internal struct GameSound
    {
        public GameSound(string Path, int RPM)
        {
            this.Path = Path;
            this.RPM = RPM;
        }

        public string Path;
        public int RPM;

        public override string ToString()
        {
            return "[\"" + Path + "\", " + RPM + "]";
        }
    }
}
