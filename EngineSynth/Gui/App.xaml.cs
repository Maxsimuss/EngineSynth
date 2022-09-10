using EngineSynth.Gui.Model;
using EngineSynth.Gui.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EngineSynth.Gui
{
    public partial class App : Application
    {
        private ApplicationModel applicationModel;

        public App()
        {
            applicationModel = new ApplicationModel();
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(applicationModel)
            };
            MainWindow.Show();
        }
    }
}
