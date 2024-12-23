using MusicVideoJukebox.Core;
using System.IO;
using System.Windows;
using Xabe.FFmpeg;

namespace MusicVideoJukebox
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Show();
            string directoryWithFFmpegAndFFprobe = Path.Combine("ffmpeg");
            FFmpeg.SetExecutablesPath(directoryWithFFmpegAndFFprobe);
            
        }
    }
}
