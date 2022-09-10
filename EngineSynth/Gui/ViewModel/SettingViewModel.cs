using EngineSynth.Gui.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Gui.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        Setting setting;

        public string Name { get => setting.Name; }

        public double Value 
        { 
            get { return setting.Value; }
            set 
            {
                setting.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public double Min { get => setting.Min; }
        public double Max { get => setting.Max; }
        public double Increment { get => setting.Increment; }

        public SettingViewModel(Setting s)
        {
            setting = s;
        }
    }
}
