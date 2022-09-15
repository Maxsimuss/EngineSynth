using EngineSynth.Model;
using EngineSynth.ViewModel;
using System.Windows;

namespace EngineSynth.View
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
