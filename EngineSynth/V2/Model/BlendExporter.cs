using CSCore;
using CSCore.Codecs.WAV;
using EngineSynth.Common.Game;
using System.IO;

namespace EngineSynth.V2.Model
{
    internal class BlendExporter
    {
        public string ExportPath { get; set; } = string.Empty;
        public string Name { get; set; } = "I6_2";
        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnStateChanged?.Invoke();
            }
        }

        public event OnStateChangedEvent OnStateChanged;
        public delegate void OnStateChangedEvent();

        public void ExportSoundBlend(Synth offLoadSynth, Synth onLoadSynth)
        {
            SfxBlendBuilder builder = new SfxBlendBuilder();

            int rpm = 250;

            string artDir = "/art/sound/engine/" + Name + "/";
            if (!Directory.Exists(ExportPath + artDir))
                Directory.CreateDirectory(ExportPath + artDir);

            while (rpm < 10000)
            {
                WriteSample(rpm, artDir, offLoadSynth, builder, SoundType.OffLoad);
                WriteSample(rpm, artDir, onLoadSynth, builder, SoundType.OnLoad);

                rpm += 250;
            }

            string jsonPath = ExportPath + "/art/sound/blends/";
            if (!Directory.Exists(jsonPath))
                Directory.CreateDirectory(jsonPath);
            File.WriteAllText(jsonPath + Name + ".sfxBlend2D.json", builder.Build());
        }

        private void WriteSample(int rpm, string artDir, Synth synth, SfxBlendBuilder builder, SoundType load)
        {
            string fileName = rpm + load.Suffix + ".wav";
            string wavPath = ExportPath + artDir + fileName;
            float[] sound = synth.GenerateLoopForRpm(rpm).Buffer;

            using (WaveWriter waveWriter = new WaveWriter(wavPath, new WaveFormat(44100, 32, 1, AudioEncoding.IeeeFloat)))
            {
                waveWriter.WriteSamples(sound, 0, sound.Length);
            }

            builder.Add(load, artDir + fileName, rpm);
        }
    }
}
