using EngineSynth.Gui.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EngineSynth.Config
{
    class Config
    {
        public List<string> Samples = new List<string>();
        public List<FilterSetting> Filters = new List<FilterSetting>();
        public Dictionary<string, double> Settings = new Dictionary<string, double>();

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;

            string json = File.ReadAllText(path);
            Config cfg = (Config)JsonConvert.DeserializeObject(json, typeof(Config));

            Samples = cfg.Samples;
            Filters = cfg.Filters;
            Settings = cfg.Settings;

            return true;
        }
    }
}
