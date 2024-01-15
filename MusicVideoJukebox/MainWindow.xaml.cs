using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MusicVideoJukebox
{
    public partial class MainWindow : Window, IMediaPlayer
    {
        MainWindowViewModel vm;
        private readonly List<UIElement> triggerElements = new List<UIElement>();


        public MainWindow()
        {
            InitializeComponent();

            triggerElements.Add(player);

            vm = new MainWindowViewModel(this);
            DataContext = vm;
        }

        public void Pause() => player.Pause();
        public void Play() => player.Play();
        public void Stop() => player.Stop();

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
            return dep != null && dep is UIElement && triggerElements.Contains(dep as UIElement);
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
    }
}
