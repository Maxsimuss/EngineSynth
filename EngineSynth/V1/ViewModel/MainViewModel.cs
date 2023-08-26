using CommunityToolkit.Mvvm.ComponentModel;
using EngineSynth.v1.Model;

namespace EngineSynth.ViewModel
{
    [INotifyPropertyChanged]
    internal partial class MainViewModel
    {
        private ApplicationModel applicationModel;

        private SynthViewModel synthViewModel;
        public SynthViewModel SynthViewModel
        {
            get => synthViewModel;
            set
            {
                synthViewModel = value;
                OnPropertyChanged(nameof(SynthViewModel));
            }
        }

        public MainViewModel(ApplicationModel applicationModel)
        {
            this.applicationModel = applicationModel;
            SynthViewModel = new SynthViewModel(applicationModel.Synth);
        }
    }
}
