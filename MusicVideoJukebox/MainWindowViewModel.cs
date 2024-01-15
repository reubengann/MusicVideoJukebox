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
        DispatcherTimer timer;

        public MainWindowViewModel(IMediaPlayer mediaPlayer)
        {
            this.mediaPlayer = mediaPlayer;
            mediaPlayer.SetSource(new System.Uri("E:\\Videos\\Music Videos\\On Media Center\\10,000 Maniacs - Because The Night [Unplugged].mp4"));
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(VideoLengthSeconds));
        }

        public ICommand PlayCommand => new DelegateCommand(PlayVideo);
        public ICommand PauseCommand => new DelegateCommand(PauseVideo);
        public ICommand StopCommand => new DelegateCommand(StopVideo);

        private void PlayVideo()
        {
            mediaPlayer.Play();
            timer.Start();
            OnPropertyChanged(nameof(VideoLengthSeconds));
            OnPropertyChanged(nameof(VideoPositionTime));
        }
        private void PauseVideo()
        {
            mediaPlayer.Pause();
            timer.Stop();
        }

        private void StopVideo()
        {
            mediaPlayer.Stop();
            timer.Stop();
        }
        public double VideoLengthSeconds => mediaPlayer.LengthSeconds;
        public double VideoPositionTime
        {
            get
            {
                return mediaPlayer.CurrentTime;
            }
            set { }
        }
    }
}
