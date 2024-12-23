using System.Windows;
using Unosquare.FFME;

namespace MusicVideoJukebox
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Unosquare.FFME.Library.FFmpegDirectory = @"C:\repos\MusicVideoJukebox\MusicVideoJukebox\ffmpeg";
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Show();

        }
    }
}
