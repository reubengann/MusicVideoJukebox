using Microsoft.Extensions.Hosting;
using System.Windows;

namespace MusicVideoJukebox
{
    public partial class App : Application
    {
        private IHost _host = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new NewMainWindow();
            MainWindow.Show();
        }
    }
}
