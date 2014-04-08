using System;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using BenchmarkDepot.Classes.GUI.ViewModel;

namespace BenchmarkDepot.Classes.GUI
{
    /// <summary>
    /// Interaction logic for EvolutionWindow.xaml
    /// </summary>
    public partial class EvolutionWindow
    {
        public EvolutionWindow(EvolutionWindowViewModel vm)
        {
            InitializeComponent();

            // set the datacontext and hook up to the closing event through view model
            // not a clean solution but the only way to gain ability to prevent closing without an external dll
            DataContext = vm;
            Closing += vm.OnViewClosing;
        }

        private void CloseCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }

}
