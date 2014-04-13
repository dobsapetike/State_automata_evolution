using System;
using System.Runtime.InteropServices;

namespace BenchmarkDepot.Classes.Misc
{

    /// <summary>
    /// Static class for providing console window commands
    /// </summary>
    public static class ConsoleManager
    {

        #region Console commands

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        [DllImport("kernel32")]
        public static extern bool AllocConsole();

        [DllImport("kernel32")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        public static void ShowConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_SHOW);
        }

        public static void HideConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        #endregion

    }

}
