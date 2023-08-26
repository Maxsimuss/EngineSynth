using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EngineSynth.v1.Model;
using System.Windows.Input;

namespace EngineSynth.ViewModel
{
    [INotifyPropertyChanged]
    public partial class ResonanceSettingViewModel
    {
        private SynthViewModel synthViewModel;
        private SynthModel synthModel;
        private ResonanceSetting resonanceSetting;
        public float Multiplier
        {
            get => resonanceSetting.Multiplier;
            set
            {
                resonanceSetting.Multiplier = value;
                OnPropertyChanged();
            }
        }

        public float Gain
        {
            get => resonanceSetting.Gain;
            set
            {
                resonanceSetting.Gain = value;
                OnPropertyChanged();
            }
        }


        [RelayCommand]
        void RemoveResonance()
        {
            synthModel.RemoveResonance(resonanceSetting);
        }

        public ICommand AddResonanceCommand { get => synthViewModel.AddResonanceCommand; }

        public ResonanceSettingViewModel(ResonanceSetting resonanceSetting, SynthViewModel synthViewModel, SynthModel synthModel)
        {
            this.resonanceSetting = resonanceSetting;
            this.synthModel = synthModel;
            this.synthViewModel = synthViewModel;
        }
    }
}
