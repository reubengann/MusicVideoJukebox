using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicVideoJukebox.Core;
using System.Windows;

namespace MusicVideoJukebox
{
    public partial class App : Application
    {
        private IHost _host = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _host = Host.CreateDefaultBuilder(e.Args).ConfigureServices(ConfigureServices).Build();
            await _host.StartAsync();

            //var mainWindow = _host.Services.GetRequiredService<MainWindow>();

            //MainWindow = mainWindow;
            //await mainWindow.Initialize();
            MainWindow = new NewMainWindow();
            MainWindow.Show();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            //services.AddSingleton<MainWindow>();
            //services.AddSingleton<SettingsWindow>();
            //services.AddSingleton<MainWindowViewModel>();
            //services.AddSingleton<ImportViewModel>();
            //services.AddSingleton<SettingsWindowViewModel>();
            //services.AddSingleton<VideoLibraryStore>();
            //services.AddSingleton<IFileSystemService, FileSystemService>();
            //services.AddSingleton<IAppSettingsFactory, FileAppSettingsFactory>();
            //services.AddSingleton<IUIThreadTimerFactory, DispatcherUITimerFactory>();
            //services.AddSingleton<IVideoLibraryBuilder, SqliteVideoLibraryBuilder>();
            //services.AddSingleton<ISettingsWindowFactory, WindowsSettingsWindowFactory>();
            //services.AddSingleton<IDialogService, WindowsDialogService>();
        }
    }
}
