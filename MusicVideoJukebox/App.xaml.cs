using MusicVideoJukebox.Impls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
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
            PowerManagement.PreventSleep();
            Exit += OnApplicationExit;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            PowerManagement.AllowSleep();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            PowerManagement.AllowSleep();
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            PowerManagement.AllowSleep();
        }
    }
}
