using CommunityToolkit.Mvvm.ComponentModel;
using EngineSynth.v1.Model;

namespace EngineSynth.ViewModel
{
    [INotifyPropertyChanged]
    public partial class SettingViewModel
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
