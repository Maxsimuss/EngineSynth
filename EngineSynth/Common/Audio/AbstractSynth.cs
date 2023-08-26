namespace EngineSynth.Common.Audio
{
    internal abstract class AbstractSynth
    {
        public float PowerstrokesPerRevolution = 3;
        public int CylinderCount = 6;

        public abstract Sample GenerateLoopForRpm(int rpm, int loopLength = 32, int minTime = 4000, bool deterministic = true);
    }
}
