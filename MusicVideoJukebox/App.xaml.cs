using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.ViewModels;
using System;
using System.IO;
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

            var maindb = _host.Services.GetRequiredService<ILibrarySetRepo>();
            await maindb.Initialize();

            MainWindow = _host.Services.GetRequiredService<NewMainWindow>();
            MainWindow.Show();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "music_video_libraries.db");
            services.AddSingleton<ILibrarySetRepo>(x => new LibrarySetRepo(dbPath));

            services.AddSingleton<NewMainWindow>();
            services.AddSingleton<NewMainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<LibraryViewModel>();
            services.AddSingleton<MetadataEditViewModel>();
            services.AddSingleton<NewPlaylistViewModel>();
            services.AddSingleton<IWindowLauncher, WindowLauncher>();
            services.AddSingleton<IMetadataManagerFactory, MetadataManagerFactory>();
            services.AddSingleton<IVideoRepoFactory, VideoRepoFactory>();
            services.AddSingleton<LibraryStore>();
            //services.AddSingleton<MainWindow>();
            //services.AddSingleton<SettingsWindow>();
            //services.AddSingleton<MainWindowViewModel>();
            //services.AddSingleton<ImportViewModel>();
            //services.AddSingleton<SettingsWindowViewModel>();
            //services.AddSingleton<VideoLibraryStore>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            //services.AddSingleton<IAppSettingsFactory, FileAppSettingsFactory>();
            //services.AddSingleton<IUIThreadTimerFactory, DispatcherUITimerFactory>();
            //services.AddSingleton<IVideoLibraryBuilder, SqliteVideoLibraryBuilder>();
            //services.AddSingleton<ISettingsWindowFactory, WindowsSettingsWindowFactory>();
            services.AddSingleton<IDialogService, WindowsDialogService>();
        }
    }
}
