using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EngineSynth.v1.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace EngineSynth.ViewModel
{
    public class ReorderableListViewModel : ObservableCollection<SampleViewModel>
    {
        private List<string> list;
        public ReorderableListViewModel(List<string> list) : base()
        {
            foreach (var i in list.Select(s => new SampleViewModel(s, this)).ToList())
            {
                Add(i);
            }

            this.list = list;
            CollectionChanged += ReorderableListViewModel_CollectionChanged;
        }

        private void ReorderableListViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                list.RemoveAt(e.OldStartingIndex);
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                list.Insert(e.NewStartingIndex, ((SampleViewModel)e.NewItems[0]).Path);
            }
        }
    }

    [INotifyPropertyChanged]
    public partial class SynthViewModel
    {
        private SynthModel synthModel;

        public string EngineName
        {
            get => synthModel.Name;
            set
            {
                synthModel.Name = value;
                OnPropertyChanged(nameof(EngineName));
            }
        }

        public ReorderableListViewModel Samples { get => new ReorderableListViewModel(synthModel.Samples); }
        public List<SettingViewModel> Settings { get => synthModel.Settings.Select(s => new SettingViewModel(s)).ToList(); }
        public List<FilterSettingViewModel> Filters { get => synthModel.Filters.Select(s => new FilterSettingViewModel(s, this, synthModel)).ToList(); }
        public List<ResonanceSettingViewModel> Resonances { get => synthModel.Resonances.Select(s => new ResonanceSettingViewModel(s, this, synthModel)).ToList(); }

        public bool IsDone { get => !synthModel.IsBusy; }

        [ObservableProperty]
        bool isEqShown = true;

        [RelayCommand]
        private void Render()
        {
            if (synthModel.ExportPath != string.Empty || TrySelectPath())
            {
                if (!synthModel.IsValid())
                {
                    MessageBox.Show("Invalid configuration! Check your settings!");
                }
                else
                {
                    synthModel.Render();
                }
            }
        }

        [RelayCommand]
        void Load()
        {
            if (synthModel.ExportPath != string.Empty || TrySelectPath())
            {
                synthModel.LoadConfig();
            }
        }

        [RelayCommand]
        void Save()
        {
            if (synthModel.ExportPath != string.Empty || TrySelectPath())
            {
                synthModel.SaveConfig();
            }
        }

        [RelayCommand]
        void SelectPath()
        {
            TrySelectPath();
        }

        [RelayCommand]
        void LoadSample()
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog dlg = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.Multiselect = true;
            if ((bool)dlg.ShowDialog())
            {
                synthModel.Samples.AddRange(dlg.FileNames);
                OnPropertyChanged(nameof(Samples));
            }
        }

        private bool TrySelectPath()
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dlg = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BeamNG.drive\";
            List<string> dirs = Directory.EnumerateDirectories(folder).ToList();

            Dictionary<double, string> map = new Dictionary<double, string>();
            foreach (var item in dirs)
            {
                string itm = item.Substring(item.LastIndexOf(Path.DirectorySeparatorChar)).Replace(Path.DirectorySeparatorChar, ' ');
                map.Add(double.Parse(itm), item);
            }
            folder = map.OrderByDescending(key => key.Key).First().Value;


            dlg.SelectedPath = folder + @"\mods\";
            if ((bool)dlg.ShowDialog())
            {
                synthModel.ExportPath = dlg.SelectedPath;
                return true;
            }

            return false;
        }
        [RelayCommand]
        void AddFilter()
        {
            synthModel.AddFilter();
        }

        [RelayCommand]
        void AddResonance()
        {
            synthModel.AddResonance();
        }

        [RelayCommand]
        void ShowEq()
        {
            IsEqShown = true;
        }

        [RelayCommand]
        void HideEq()
        {
            IsEqShown = false;
        }

        public void FileDrop(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;

            foreach (string file in files)
            {
                synthModel.Samples.Add(file);
            }
            OnPropertyChanged(nameof(Samples));
        }

        public SynthViewModel(SynthModel model)
        {
            synthModel = model;

            //memory leak
            synthModel.OnStateChanged += () => OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(null));

            //RenderCommand = new RenderCommand(synthModel);
            //SelectPathCommand = new SetExportPathCommand(synthModel);
            //AddFilterCommand = new AddFilterCommand(synthModel);
        }
    }
}
