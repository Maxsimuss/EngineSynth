using EngineSynth.V2.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace EngineSynth.V2.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new V2MainViewModel();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is V2MainViewModel)
            {
                ((V2MainViewModel)DataContext).EngineNameChanged(((TextBox)sender).Text);
            }
        }

        private void SamplePanelFileDrop(object sender, DragEventArgs e)
        {
            if (DataContext is V2MainViewModel)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files == null) return;

                foreach (string file in files)
                {
                    ((V2MainViewModel)DataContext).AddSample(file);
                }
            }
        }
    }
}
