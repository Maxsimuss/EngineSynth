using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EngineSynth.Model;
using System.Windows.Input;

namespace EngineSynth.ViewModel
{
    [INotifyPropertyChanged]
    public partial class FilterSettingViewModel
    {
        private SynthViewModel synthViewModel;
        private SynthModel synthModel;
        private FilterSetting filterSetting;
        public float Frequency
        {
            get => filterSetting.Frequency;
            set
            {
                filterSetting.Frequency = value;
                OnPropertyChanged();
            }
        }

        public float Q
        {
            get => filterSetting.Q;
            set
            {
                filterSetting.Q = value;
                OnPropertyChanged();
            }
        }

        public float Gain
        {
            get => filterSetting.Gain;
            set
            {
                filterSetting.Gain = value;
                OnPropertyChanged();
            }
        }


        [RelayCommand]
        void RemoveFilter()
        {
            synthModel.RemoveFilter(filterSetting);
        }

        public ICommand AddFilterCommand { get => synthViewModel.AddFilterCommand; }

        public FilterSettingViewModel(FilterSetting filterSetting, SynthViewModel synthViewModel, SynthModel synthModel)
        {
            this.filterSetting = filterSetting;
            this.synthModel = synthModel;
            this.synthViewModel = synthViewModel;
        }
    }
}
