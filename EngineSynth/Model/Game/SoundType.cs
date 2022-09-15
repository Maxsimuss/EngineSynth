namespace EngineSynth.Model.Game
{
    internal class SoundType
    {
        public static readonly SoundType OffLoad = new SoundType("");
        public static readonly SoundType OnLoad = new SoundType("_P");

        public string Suffix;

        public SoundType(string suffix)
        {
            Suffix = suffix;
        }
    }
}
