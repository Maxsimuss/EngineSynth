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

namespace EngineSynth.Gui
{
    /// <summary>
    /// Interaction logic for DraggableElemnt.xaml
    /// </summary>
    public partial class DraggableElement : UserControl
    {
        public int Index = 0;
        public string File;
        public DraggableElement(string File, int Index = 0)
        {
            InitializeComponent();
            this.Index = Index;
            this.File = File;
            TextBlock.Text = System.IO.Path.GetFileName(File);
        }
    }
}
