using System;
using System.Windows.Forms;
using GameHub.Services;

namespace GameHub
{
    internal static class Program
    {
        /// <summary>Holds the currently authenticated user.</summary>
        public static Models.User CurrentUser;

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ApplicationExit += delegate { ImageCacheService.Clear(); GameAssetService.ClearGenerated(); };
            Application.Run(new Forms.LoginForm());
        }
    }
}
