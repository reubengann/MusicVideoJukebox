using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            await mainWindow.Initialize();

            //var appSettingsStore = await AppSettingsStore.Create();

            //if (string.IsNullOrWhiteSpace(appSettingsStore.VideoLibraryPath) || !Directory.Exists(appSettingsStore.VideoLibraryPath))
            //{
            //    var dialog = new OpenFolderDialog
            //    {
            //        Title = "Select a folder for the Video Library",
            //        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            //    };

            //    if (dialog.ShowDialog() == true)
            //    {
            //        // Update the path in settings
            //        appSettingsStore.UpdateVideoLibraryPath(dialog.FolderName);

            //        // Save updated settings
            //        await appSettingsStore.Save();
            //    }
            //    else
            //    {
            //        MessageBox.Show("A folder is required to continue. The application will now exit.",
            //                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //        Application.Current.Shutdown();
            //        return;
            //    }
            //}

            //ArgumentNullException.ThrowIfNull(appSettingsStore.VideoLibraryPath);
            //var libraryStore = new VideoLibraryStore(await VideoLibraryBuilder.BuildAsync(appSettingsStore.VideoLibraryPath));
            //MainWindow = new MainWindow(libraryStore);

            MainWindow.Show();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            //services.AddSingleton<IFolderPicker, WpfFolderPicker>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }
}
