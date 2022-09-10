using EngineSynth.Gui.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSynth.Gui.ViewModel
{
    internal class MainViewModel : ViewModelBase
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
