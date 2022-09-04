using EngineSynth.Gui.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EngineSynth.Gui.View
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
