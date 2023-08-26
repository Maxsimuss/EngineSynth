namespace EngineSynth.Common.Game
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
