using System;
using System.Windows;

namespace MusicVideoJukebox
{
    public partial class MainWindow : Window, IMediaPlayer
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Play()
        {
            player.Play();
        }

        public void Stop()
        {
            player.Stop();
        }

        public void SetSource(Uri source)
        {
            player.Source = source;
        }

        public double LengthSeconds
        {
            get
            {
                if (player.NaturalDuration.HasTimeSpan)
                    return player.NaturalDuration.TimeSpan.TotalSeconds;
                return 1;
            }
        }

        public double CurrentTime => player.Position.TotalSeconds;
    }
}
