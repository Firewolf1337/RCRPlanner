using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RCRPlanner
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            // Application is running
            // Process command line args
            bool startMinimized = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/startMinimized")
                {
                    mainWindow.WindowState = WindowState.Minimized;
                }
                if (e.Args[i] == "/noAutostart" )
                {
                    mainWindow.autostartsuppress = true;
                }
            }
            mainWindow.Show();
        }
    }
}
