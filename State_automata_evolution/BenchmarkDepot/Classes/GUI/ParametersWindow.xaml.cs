using System;
using System.Windows.Input;

namespace BenchmarkDepot.Classes.GUI
{
    /// <summary>
    /// Interaction logic for ParametersWindow.xaml
    /// </summary>
    public partial class ParametersWindow
    {
        public ParametersWindow()
        {
            InitializeComponent();
        }

        private void CloseCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }

}