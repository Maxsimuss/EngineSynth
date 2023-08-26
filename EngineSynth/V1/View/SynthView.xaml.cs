using EngineSynth.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace EngineSynth.View
{
    /// <summary>
    /// Interaction logic for SynthView.xaml
    /// </summary>
    public partial class SynthView : UserControl
    {
        public SynthView()
        {
            InitializeComponent();
        }

        //
        // Breaking the MVVM principle
        //
        private void SamplePanelFileDrop(object sender, DragEventArgs e)
        {
            ((SynthViewModel)this.DataContext).FileDrop(e);
        }
    }
}
