using System.Threading;
using System.Windows;

namespace X330Backlight
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Mutex mutex = new Mutex(true, "X330Backlight", out var isRunning);
            if (isRunning)
            {
                MainWindow = new MainWindow();
                MainWindow.ShowDialog();
                mutex.ReleaseMutex();
            }
            else
            {
                Shutdown(0);
            }
        }
    }
}
