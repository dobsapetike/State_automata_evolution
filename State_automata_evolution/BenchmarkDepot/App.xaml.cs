using BenchmarkDepot.Classes.Core;
using System;
using System.Windows;

namespace BenchmarkDepot
{

    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            if (e.Args.Length == 2 && e.Args[0] == "console")
            {
                // start console mode
                var quickRun = new QuickStartSetup(e.Args[1]);
                quickRun.Start();
                Shutdown();
            }

            // otherwise run the gui application
            StartupUri = new Uri("Classes/GUI/MainWindow.xaml", UriKind.Relative);
        }

    }

}
