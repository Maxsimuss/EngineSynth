using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EngineSynth.Common.Util;
using EngineSynth.V2.Model;
using EngineSynth.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace EngineSynth.V2.ViewModel
{
    internal partial class V2MainViewModel : ObservableObject
    {
        public float CurrentRPM
        {
            get => player.RPM;
            set
            {
                player.RPM = value;
                OnPropertyChanged(nameof(CurrentRPM));
            }
        }

        private string modPath = "";
        private string engineName = "MyEngine";
        private string configSavePath = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProjectDisplayName))]
        private string projectName = "None";
        public string ProjectDisplayName => "Project: " + ProjectName;

        private Synth offLoadSynth, onLoadSynth;

        public string LoadLabel => loadState ? "On Load" : "Off Load";
        private bool loadState
        {
            get => player.Load;
            set
            {
                player.Load = value;
                OnPropertyChanged(nameof(Automations));
                OnPropertyChanged(nameof(Samples));
                OnPropertyChanged(nameof(LoadLabel));
            }
        }

        public List<Automation> Automations => (loadState ? onLoadSynth : offLoadSynth).Automations;
        public ReorderableListViewModel Samples
        {
            get
            {
                ReorderableListViewModel list = new ReorderableListViewModel((loadState ? onLoadSynth : offLoadSynth).SamplePaths);
                list.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
                {
                    (loadState ? onLoadSynth : offLoadSynth).CylinderCount = (loadState ? onLoadSynth : offLoadSynth).SamplePaths.Count;
                    (loadState ? onLoadSynth : offLoadSynth).PowerstrokesPerRevolution = (loadState ? onLoadSynth : offLoadSynth).SamplePaths.Count / 2f;
                };

                return list;
            }
        }

        private RealtimePlayer player;
        public V2MainViewModel()
        {
            offLoadSynth = new Synth();
            offLoadSynth.CylinderCount = 10;
            offLoadSynth.PowerstrokesPerRevolution = 5;

            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick3_Lust\WAV\TMKD03 - lust_kick_05.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_02.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick3_Lust\WAV\TMKD03 - lust_kick_04.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_03.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick3_Lust\WAV\TMKD03 - lust_kick_03.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_04.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick3_Lust\WAV\TMKD03 - lust_kick_02.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_05.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick3_Lust\WAV\TMKD03 - lust_kick_01.wav");
            offLoadSynth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_06.wav");

            onLoadSynth = new Synth();
            onLoadSynth.SamplePaths = offLoadSynth.SamplePaths;
            onLoadSynth.CylinderCount = offLoadSynth.CylinderCount;
            onLoadSynth.PowerstrokesPerRevolution = offLoadSynth.PowerstrokesPerRevolution;

            offLoadSynth.Init();
            onLoadSynth.Init();
            onLoadSynth.randomSeed = offLoadSynth.randomSeed;

            player = new RealtimePlayer(offLoadSynth, onLoadSynth);
            player.RPM = 5000;
        }

        [RelayCommand]
        void StartPreview()
        {
            player.Start();
        }

        [RelayCommand]
        void StopPreview()
        {
            player.Stop();
        }

        [RelayCommand]
        void ToggleAutoPreview()
        {
            loadState = !loadState;
        }

        [RelayCommand]
        void SelectModFolder()
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
                modPath = dlg.SelectedPath;
                ProjectName = modPath.Split(Path.DirectorySeparatorChar).Last();
            }
        }

        [RelayCommand]
        void SaveConfigAs()
        {
            SaveConfig(false);
        }

        [RelayCommand]
        void SaveConfig()
        {
            SaveConfig(true);
        }

        void SaveConfig(bool usePreviousPath)
        {
            if (configSavePath.Length < 1 || !usePreviousPath)
            {
                Ookii.Dialogs.Wpf.VistaSaveFileDialog dlg = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
                string folder = modPath;
                if (folder.Length < 1)
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BeamNG.drive\";

                    List<string> dirs = Directory.EnumerateDirectories(folder).ToList();

                    Dictionary<double, string> map = new Dictionary<double, string>();
                    foreach (var item in dirs)
                    {
                        string itm = item.Substring(item.LastIndexOf(Path.DirectorySeparatorChar)).Replace(Path.DirectorySeparatorChar, ' ');
                        map.Add(double.Parse(itm), item);
                    }
                    folder = map.OrderByDescending(key => key.Key).First().Value + @"\mods\";
                }


                dlg.FileName = folder + "/" + engineName;
                dlg.DefaultExt = ".ecfg";

                if ((bool)dlg.ShowDialog())
                {
                    configSavePath = dlg.FileName;
                    SaveConfig(configSavePath);
                }
            }
            else
            {
                SaveConfig(configSavePath);
            }
        }

        private void SaveConfig(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(new SynthConfig[] { new SynthConfig(offLoadSynth), new SynthConfig(onLoadSynth) }));
        }

        [RelayCommand]
        void LoadConfig()
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog dlg = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            string folder = modPath;
            if (folder.Length < 1)
            {
                folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BeamNG.drive\";

                List<string> dirs = Directory.EnumerateDirectories(folder).ToList();

                Dictionary<double, string> map = new Dictionary<double, string>();
                foreach (var item in dirs)
                {
                    string itm = item.Substring(item.LastIndexOf(Path.DirectorySeparatorChar)).Replace(Path.DirectorySeparatorChar, ' ');
                    map.Add(double.Parse(itm), item);
                }
                folder = map.OrderByDescending(key => key.Key).First().Value + @"\mods\";
            }

            dlg.FileName = folder + "/" + engineName;
            dlg.DefaultExt = ".ecfg";

            if ((bool)dlg.ShowDialog())
            {
                List<SynthConfig> configs = JsonConvert.DeserializeObject<List<SynthConfig>>(File.ReadAllText(dlg.FileName));
                configs[0].ApplyTo(offLoadSynth);
                configs[1].ApplyTo(onLoadSynth);
                OnPropertyChanged(nameof(Automations));
                OnPropertyChanged(nameof(Samples));
            }
        }

        public void EngineNameChanged(string name)
        {
            engineName = name;
        }

        [RelayCommand]
        void Render()
        {
            if (modPath.Length < 1)
            {
                SelectModFolder();
            }

            if (modPath.Length < 1) return;

            BlendExporter blendExporter = new BlendExporter();
            blendExporter.ExportPath = modPath;
            blendExporter.Name = engineName;

            blendExporter.ExportSoundBlend(offLoadSynth, onLoadSynth);
        }

        [RelayCommand]
        void Minimize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        void Maximize()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        [RelayCommand]
        void Exit()
        {
            Environment.Exit(0);
        }

        public void AddSample(string file)
        {
            (loadState ? onLoadSynth : offLoadSynth).SamplePaths.Add(file);

            (loadState ? onLoadSynth : offLoadSynth).CylinderCount = (loadState ? onLoadSynth : offLoadSynth).SamplePaths.Count;
            (loadState ? onLoadSynth : offLoadSynth).PowerstrokesPerRevolution = (loadState ? onLoadSynth : offLoadSynth).SamplePaths.Count / 2f;
        }
    }
}
