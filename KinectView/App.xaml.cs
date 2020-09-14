
using KinectComputerVision;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KinectView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly FrameController frameController;

        public App()
        {
            this.frameController = new FrameController();
        }

        void App_Startup(object sender, StartupEventArgs e)
        {

            MainWindow mainWindow = new MainWindow(this.frameController);

            mainWindow.Show();
        }


    }
}
