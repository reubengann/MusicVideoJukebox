using MusicVideoJukebox.Core;
using System.Windows;

namespace MusicVideoJukebox
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            var store = new VideoLibraryStore(await AllSongsShuffledVideoLibraryBuilder.Build("E:\\Videos\\Music Videos\\On Media Center"));
            MainWindow = new MainWindow(store);
            MainWindow.Show();
        }
    }
}
