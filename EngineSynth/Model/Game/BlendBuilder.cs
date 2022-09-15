using System.Collections.Generic;

namespace EngineSynth.Model.Game
{
    internal class SfxBlendBuilder
    {


        List<GameSound> soundsOn = new List<GameSound>();
        List<GameSound> soundsOff = new List<GameSound>();

        public void Add(SoundType type, string path, int rpm)
        {
            if (type == SoundType.OnLoad)
            {
                soundsOn.Add(new(path, rpm));
            }
            else
            {
                soundsOff.Add(new(path, rpm));
            }
        }

        public string Build()
        {
            string json =
                $$"""
                {
                    "header": {
                        "version": 1
                    },
                    "eventName": "event:>Engine>default",
                    "samples": 
                    [
                        [

                """;

            foreach (GameSound sound in soundsOff)
            {
                json += "            " + sound.ToString() + ",\n";
            }

            json +=
    $$"""
                        ],
                        [

                """;

            foreach (GameSound sound in soundsOn)
            {
                json += "            " + sound.ToString() + ",\n";
            }

            json +=
                $$"""
                        ]
                    ]
                }
                """;

            return json;
        }
    }
}
