using EngineSynth.Audio;
using EngineSynth.Audio.Synth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EngineSynth.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string Path = null;

        public MainWindow()
        {
            InitializeComponent();

            FilePanel.Drop += FilePanelDrop;
            FilePanel.MouseMove += FilePanelMouseMove;
            FilePanel.MouseUp += DraggableMouseUp;
        }

        DraggableElement holding = null;
        private void FilePanelMouseMove(object sender, MouseEventArgs e)
        {
            if (holding == null) return;

            double y = e.GetPosition(FilePanel).Y;
            int idx = Math.Clamp((int)(y / (holding.Height + holding.Margin.Top)), 0, FilePanel.Children.Count - 1);

            if(holding.Index != idx)
            {
                FilePanel.Children.Remove(holding);
                FilePanel.Children.Insert(idx, holding);
                
                for (int i = 0; i < FilePanel.Children.Count; i++)
                {
                    ((DraggableElement)FilePanel.Children[i]).Index = i;
                }
            }
        }

        private void DraggableMouseUp(object sender, MouseButtonEventArgs e)
        {
            holding = null;
        }

        private void DraggableMouseDown(object sender, MouseButtonEventArgs e)
        {
            holding = (DraggableElement)sender;
        }

        private void FilePanelDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                DraggableElement block = new DraggableElement(file);
                block.MouseDown += DraggableMouseDown;
                block.MouseUp += DraggableMouseUp;
                block.Index = FilePanel.Children.Count;
                block.Remove.Click += (s, e) =>
                {
                    FilePanel.Children.Remove(block);
                    for (int i = 0; i < FilePanel.Children.Count; i++)
                    {
                        ((DraggableElement)FilePanel.Children[i]).Index = i;
                    }
                    holding = null;
                };

                FilePanel.Children.Add(block);
            }
        }

        private void RenderClick(object sender, RoutedEventArgs e)
        {
            Engine engine = new Engine();
            engine.name = NameBox.Text;
            engine.PowerstrokesPerRotation = (int)ppr.Value;
            engine.CylinderCount = (int)cylCount.Value;
            engine.FiringOrder = new int[(int)cylCount.Value];
            for (int i = 0; i < (int)cylCount.Value; i++)
            {
                engine.FiringOrder[i] = i + 1;
            }
            engine.MinRPM = (int)MinRPM.Value;
            engine.MaxRPM = (int)MaxRPM.Value;
            engine.RPMInc = (int)RPMIncrements.Value;
            engine.OnGain = (float)OnGain.Value;
            engine.OffGain = (float)OffGain.Value;
            engine.Randomness = (float)Randomness.Value;
            engine.Pitch = (float)Pitch.Value;
            engine.BaseFreqGain = (float)BaseFreqGain.Value;
            engine.FilterFreq = (float)FilterFreq.Value;
            engine.RpmGain = (float)RpmGain.Value;
            engine.OffLoadFilter = (float)OffLoadFilter.Value;


            string[] files = new string[FilePanel.Children.Count];
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = ((DraggableElement)FilePanel.Children[i]).File;
            }

            SynthSettings settings = new SynthSettings();
            for (int i = 1; i < FilterPanel.Children.Count; i++)
            {
                PeakingFilterControl c = (PeakingFilterControl)FilterPanel.Children[i];
                settings.filters.Add(new FilterData(c.Freq.Value, 1 / Math.Max(Math.Pow(c.Q.Value, 2), 0.001), c.Gain.Value));
            }

            if(Path != null || Select())
            {
                ((Button)sender).IsEnabled = false;
                Synth synth = new Synth(engine, settings, Path);
                Task.Run(() => synth.Run(files)).ContinueWith((e) => Dispatcher.Invoke(() => ((Button)sender).IsEnabled = true));
            }
        }

        private void FilterPanelClick(object sender, MouseButtonEventArgs e)
        {
            StackPanel s = (StackPanel)sender;
            if(s.Children.Count < 2)
            {
                s.Children[0].Visibility = Visibility.Collapsed;
                s.HorizontalAlignment = HorizontalAlignment.Left;
                s.Children.Add(new PeakingFilterControl());
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            Config.Config cfg = new Config.Config();
            if(!cfg.Load(NameBox.Text + ".ecfg")) return;

            foreach (var item in cfg.Settings)
            {
                ((Slider)FindName(item.Key)).Value = item.Value;
            }

            FilePanel.Children.Clear();
            for (int i = 0; i < cfg.Samples.Count; i++)
            {
                DraggableElement block = new DraggableElement(cfg.Samples[i], i);
                block.MouseDown += DraggableMouseDown;
                block.MouseUp += DraggableMouseUp;
                block.Index = FilePanel.Children.Count;
                block.Remove.Click += (s, e) =>
                {
                    FilePanel.Children.Remove(block);
                    for (int i = 0; i < FilePanel.Children.Count; i++)
                    {
                        ((DraggableElement)FilePanel.Children[i]).Index = i;
                    }
                    holding = null;
                };

                FilePanel.Children.Add(block);
            }

            FilterPanel.Children.RemoveRange(1, FilterPanel.Children.Count - 1);
            foreach (var item in cfg.Filters)
            {
                FilterPanel.Children[0].Visibility = Visibility.Collapsed;
                FilterPanel.HorizontalAlignment = HorizontalAlignment.Left;

                var c = new PeakingFilterControl();
                c.Q.Value = item.Q;
                c.Gain.Value = item.Gain;
                c.Freq.Value = item.Frequency;

                FilterPanel.Children.Add(c);
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Config.Config cfg = new Config.Config();
            foreach (Slider item in SettingPanel.Children)
            {
                cfg.Settings.Add(item.Name, item.Value);
            }
            foreach (DraggableElement item in FilePanel.Children)
            {
                cfg.Samples.Add(item.File);
            }
            for (int i = 1; i < FilterPanel.Children.Count; i++)
            {
                PeakingFilterControl c = (PeakingFilterControl)FilterPanel.Children[i];
                cfg.Filters.Add(new FilterData(c.Freq.Value, 1 / Math.Max(Math.Pow(c.Q.Value, 2), 0.001), c.Gain.Value));
            }

            cfg.Save(NameBox.Text + ".ecfg");
        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private bool Select()
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dlg = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BeamNG.drive\";
            if ((bool)dlg.ShowDialog())
            {
                Path = dlg.SelectedPath;
                return true;
            }
            return false;
        }
    }
}
