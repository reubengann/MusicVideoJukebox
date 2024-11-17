using MusicVideoJukebox.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicVideoJukebox
{
    public partial class MainWindow : Window, IMediaPlayer
    {
        readonly MainWindowViewModel vm;
        private readonly List<UIElement> triggerElements = new();
        private bool fadingOut = false;
        private bool fadingIn = false;

        public MainWindow(ISettingsWindowFactory settingsWindowFactory, IDialogService dialogService)
        {
            InitializeComponent();
            triggerElements.Add(player);
            vm = new MainWindowViewModel(this, settingsWindowFactory, dialogService);
            DataContext = vm;
            player.MediaEnded += Player_MediaEnded;
        }

        public async Task Initialize()
        {
            await vm.Initialize();
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            vm.DonePlaying();
        }

        public void Pause() => player.Pause();
        public void Play() => player.Play();

        public void SetSource(Uri source) => player.Source = source;

        public double LengthSeconds
        {
            get
            {
                if (player.NaturalDuration.HasTimeSpan)
                    return player.NaturalDuration.TimeSpan.TotalSeconds;
                return 1;
            }
        }

        public double CurrentTimeSeconds
        {
            get => player.Position.TotalSeconds;
            set => player.Position = TimeSpan.FromSeconds(value);
        }

        private void Slider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            vm.StartScrubbing();
        }

        private void Slider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            vm.StopScrubbing();
        }

        public double Volume
        {
            get => player.Volume;
            set => player.Volume = value;
        }

        private bool IsTriggerElement(DependencyObject dep)
        {
            return dep != null && dep is UIElement && triggerElements.Contains((dep as UIElement) ?? throw new Exception());
        }

        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            if (IsTriggerElement(dep) || IsTriggerElement(VisualTreeHelper.GetParent(dep)))
            {
                vm.ChangeToFullScreenToggled();
            }
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

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            vm.UserInteracted();
        }

        public void FadeButtonsOut()
        {
            if (fadingOut) return;
            var fadeOutAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5) // Adjust the duration as needed
            };

            fadeOutAnimation.Completed += (s, e) => fadingOut = false;
            fadingOut = true;
            VideoControls.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        public void MaybeFadeButtonsIn()
        {
            if (fadingOut) fadingOut = false;
            if (fadingIn) return;

            var fadeInAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.25) // Adjust the duration as needed
            };
            fadeInAnimation.Completed += (s, e) => fadingIn = false;
            fadingIn = true;
            VideoControls.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        public void ShowInfo()
        {
            VideoInfo.BeginAnimation(OpacityProperty, new DoubleAnimation { To = 1, Duration = TimeSpan.FromSeconds(0.25) });
        }

        public void HideInfo()
        {
            VideoInfo.BeginAnimation(OpacityProperty, new DoubleAnimation { To = 0, Duration = TimeSpan.FromSeconds(0.25) });
        }
    }
}
