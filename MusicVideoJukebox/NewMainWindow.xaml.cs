using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.UserInterface;
using MusicVideoJukebox.Core.ViewModels;
using System;
using System.IO;
using System.Windows;

namespace MusicVideoJukebox
{
    public partial class NewMainWindow : Window
    {
        private readonly NewMainWindowViewModel vm;
        private readonly IFadesWhenInactive interfaceFader;
        IServiceProvider serviceProvider;

        public NewMainWindow()
        {
            InitializeComponent();
            serviceProvider = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build().Services;
            this.vm = serviceProvider.GetRequiredService<NewMainWindowViewModel>();
            interfaceFader = serviceProvider.GetRequiredService<IFadesWhenInactive>();
            player.DataContext = serviceProvider.GetRequiredService<VideoPlayingViewModel>();
            DataContext = vm;
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            interfaceFader.UserInteracted();
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
            services.AddSingleton<PlaylistEditViewModel>();
            services.AddSingleton<VideoPlayingViewModel>();
            services.AddSingleton<IWindowLauncher, WindowLauncher>();
            services.AddSingleton<IMetadataManagerFactory, MetadataManagerFactory>();
            services.AddSingleton<IVideoRepoFactory, VideoRepoFactory>();
            services.AddSingleton<LibraryStore>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<IUIThreadTimerFactory, DispatcherUITimerFactory>();
            services.AddSingleton<IDialogService, WindowsDialogService>();
            services.AddSingleton<IReferenceDataRepo>(s => new ReferenceDataRepo("reference.sqlite"));
            var playerControls = player.playerControls as FrameworkElement;
            services.AddSingleton<IFadesWhenInactive>(new InterfaceFader([Sidebar, playerControls], OpacityProperty));
            services.AddSingleton<IMediaPlayer2>(new MediaElementMediaPlayer(player.media, player.VideoInfo, OpacityProperty));
        }
    }
}
