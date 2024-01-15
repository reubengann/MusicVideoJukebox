using System;
using System.Windows;

namespace MusicVideoJukebox
{
    public partial class MainWindow : Window, IMediaPlayer
    {
        MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
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
    }
}
