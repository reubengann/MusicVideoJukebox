using Prism.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicVideoJukebox
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IMediaPlayer mediaPlayer;
        DispatcherTimer progressUpdateTimer;
        DispatcherTimer scrubDebouceTimer;
        bool isScrubbing = false;
        bool scrubbedRecently = false;
        bool isFullScreen = false;

        public MainWindowViewModel(IMediaPlayer mediaPlayer)
        {
            this.mediaPlayer = mediaPlayer;
            mediaPlayer.Volume = 1;
            mediaPlayer.SetSource(new System.Uri("E:\\Videos\\Music Videos\\On Media Center\\10,000 Maniacs - Because The Night [Unplugged].mp4"));
            progressUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            scrubDebouceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            progressUpdateTimer.Tick += Timer_Tick;
            scrubDebouceTimer.Tick += ScrubDebouceTimer_Tick;
        }

        private void ScrubDebouceTimer_Tick(object? sender, EventArgs e)
        {
            scrubbedRecently = false;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (isScrubbing) return;
            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(VideoLengthSeconds));
        }

        public ICommand PlayCommand => new DelegateCommand(PlayVideo);
        public ICommand PauseCommand => new DelegateCommand(PauseVideo);
        public ICommand StopCommand => new DelegateCommand(StopVideo);
        public double Volume
        {
            get => mediaPlayer.Volume;
            set => mediaPlayer.Volume = value;
        }

        public void StopScrubbing()
        {
            scrubDebouceTimer.Stop();
            scrubbedRecently = false;
            isScrubbing = false;
        }

        public void StartScrubbing()
        {
            scrubDebouceTimer.Start();
            isScrubbing = true;
        }

        private void PlayVideo()
        {
            mediaPlayer.Play();
            progressUpdateTimer.Start();
            OnPropertyChanged(nameof(VideoLengthSeconds));
            OnPropertyChanged(nameof(VideoPositionTime));
        }
        private void PauseVideo()
        {
            mediaPlayer.Pause();
            progressUpdateTimer.Stop();
        }

        private void StopVideo()
        {
            mediaPlayer.Stop();
            progressUpdateTimer.Stop();
        }

        public void ChangeToFullScreenToggled()
        {
            if (isFullScreen)
            {
                mediaPlayer.SetWindowed();
                isFullScreen = false;
            }
            else
            {
                mediaPlayer.SetFullScreen();
                isFullScreen = true;
            }
        }

        public double VideoLengthSeconds => mediaPlayer.LengthSeconds;
        public double VideoPositionTime
        {
            get => mediaPlayer.CurrentTimeSeconds;
            set
            {
                if (scrubbedRecently) return;
                scrubbedRecently = true;
                mediaPlayer.CurrentTimeSeconds = value;
            }
        }
    }
}
