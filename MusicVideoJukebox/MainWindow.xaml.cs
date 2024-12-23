using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.UserInterface;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Impls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicVideoJukebox
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm;
        private readonly VideoPlayingViewModel vpvm;
        private readonly IFadesWhenInactive interfaceFader;
        IServiceProvider serviceProvider;
        IKeyboardHandler keyboardHandler;
        INavigationService navigationService;
        private bool isFullScreen = false;

        public MainWindow()
        {
            InitializeComponent();
            serviceProvider = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build().Services;
            this.vm = serviceProvider.GetRequiredService<MainWindowViewModel>();
            interfaceFader = serviceProvider.GetRequiredService<IFadesWhenInactive>();
            vpvm = serviceProvider.GetRequiredService<VideoPlayingViewModel>();
            player.DataContext = vpvm;
            keyboardHandler = serviceProvider.GetRequiredService<IKeyboardHandler>();
            navigationService = serviceProvider.GetRequiredService<INavigationService>();
            KeyDown += NewMainWindow_KeyDown;
            DataContext = vm;
            player.MediaElementDoubleClicked += Player_MediaElementDoubleClicked;
        }

        private void Player_MediaElementDoubleClicked()
        {
            ToggleFullScreen();
        }

        public void ToggleFullScreen()
        {
            if (isFullScreen)
            {
                SetWindowed();
            }
            else
            {
                SetFullScreen();
            }

            isFullScreen = !isFullScreen;
        }

        public void SetWindowed()
        {
            ResizeMode = ResizeMode.CanResize;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        public void SetFullScreen()
        {
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void NewMainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (navigationService.CurrentViewModel != null) return;
            switch (e.Key)
            {
                case Key.MediaNextTrack:
                case Key.Right:
                    keyboardHandler.NextTrackPressed();
                    e.Handled = true;
                    break;
                case Key.MediaPreviousTrack:
                case Key.Left:
                    keyboardHandler.PrevTrackPressed();
                    e.Handled = true;
                    break;
                case Key.MediaPlayPause:
                case Key.Space:
                    keyboardHandler.PlayPauseKeyPressed();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    if (isFullScreen)
                    {
                        SetWindowed();
                    }
                    e.Handled = true;
                    break;
            }
        }

        protected override async void OnSourceInitialized(EventArgs e)
        {
            var repo = serviceProvider.GetRequiredService<ILibrarySetRepo>();
            await repo.Initialize();
            base.OnSourceInitialized(e);
            var store = serviceProvider.GetRequiredService<LibraryStore>();
            await store.Initialize();
            vm.RestoreState();
            await vpvm.Recheck();
        }

        private Point _lastMousePosition = new Point(-1, -1); // Initial invalid position

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // why is this needed???
            Point currentMousePosition = e.GetPosition(this);
            if (currentMousePosition == _lastMousePosition)
            {
                return;
            }
            _lastMousePosition = currentMousePosition;
            interfaceFader.UserInteracted();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "music_video_libraries.db");
            services.AddSingleton<ILibrarySetRepo>(x => new LibrarySetRepo(dbPath));

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddTransient<LibraryViewModel>();
            services.AddTransient<MetadataEditViewModel>();
            services.AddTransient<PlaylistEditViewModel>();
            services.AddTransient<PlaylistPlayViewModel>();
            services.AddTransient<AnalyzeViewModel>();
            services.AddSingleton<IDialogService>(x => new WindowsDialogService(this));
            
            services.AddSingleton<IStreamAnalyzer, StreamAnalyzer>();
            services.AddSingleton<VideoPlayingViewModel>();
            services.AddSingleton<IWindowLauncher, WindowLauncher>();
            services.AddSingleton<IMetadataManagerFactory, MetadataManagerFactory>();
            services.AddSingleton<IVideoRepoFactory, VideoRepoFactory>();
            services.AddSingleton<LibraryStore>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<IUIThreadTimerFactory, DispatcherUITimerFactory>();
            services.AddSingleton<IKeyboardHandler, KeyboardHandler>();
            services.AddSingleton<IReferenceDataRepo>(s => new ReferenceDataRepo("reference.sqlite"));
            var playerControls = player.playerControls as FrameworkElement;
            services.AddSingleton<IFadesWhenInactive>(new InterfaceFader([Sidebar, playerControls], OpacityProperty));
            services.AddSingleton<IMediaPlayer>(new MediaElementMediaPlayer(player.media, player.VideoInfo, OpacityProperty));
            services.AddSingleton<IUiThreadDispatcher, WpfUiThreadDispatcher>();
        }
    }
}
