using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EngineSynth.Gui.ViewModel
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
