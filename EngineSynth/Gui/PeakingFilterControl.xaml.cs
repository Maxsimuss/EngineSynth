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
    /// Interaction logic for PeakingFilterControl.xaml
    /// </summary>
    public partial class PeakingFilterControl : UserControl
    {
        public PeakingFilterControl()
        {
            InitializeComponent();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel p = ((StackPanel)Parent);
            p.Children.Remove(this);
            if(p.Children.Count < 2)
            {
                p.Children[0].Visibility = Visibility.Visible;
                p.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ((StackPanel)Parent).Children.Insert(((StackPanel)Parent).Children.IndexOf(this) + 1, new PeakingFilterControl());
        }
    }
}
