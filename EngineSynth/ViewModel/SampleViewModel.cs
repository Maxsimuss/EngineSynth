using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EngineSynth.ViewModel
{
    [INotifyPropertyChanged]
    public partial class SampleViewModel
    {
        private ReorderableListViewModel sampleViewModel;

        [ObservableProperty]
        string path, name;

        [RelayCommand]
        void Remove()
        {
            sampleViewModel.Remove(this);
        }

        public SampleViewModel(string path, ReorderableListViewModel sampleViewModel)
        {
            this.sampleViewModel = sampleViewModel;
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }
    }
}
