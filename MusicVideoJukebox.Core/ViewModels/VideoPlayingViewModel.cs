using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class VideoPlayingViewModel : BaseViewModel
    {
        private bool isPlaying = false;
        private readonly IMediaPlayer2 mediaPlayer;
        private readonly IUIThreadTimerFactory uIThreadTimerFactory;

        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }

        public double VideoLengthSeconds => mediaPlayer.LengthSeconds;
        public double VideoPositionTime
        {
            get => mediaPlayer.CurrentTimeSeconds;
            set
            {
                //if (scrubbedRecently) return;
                //scrubbedRecently = true;
                mediaPlayer.CurrentTimeSeconds = value;
            }
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        IUIThreadTimer progressUpdateTimer;

        public VideoPlayingViewModel(IMediaPlayer2 mediaElementMediaPlayer, IUIThreadTimerFactory uIThreadTimerFactory)
        {
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            this.mediaPlayer = mediaElementMediaPlayer;
            this.uIThreadTimerFactory = uIThreadTimerFactory;
            progressUpdateTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            progressUpdateTimer.Tick += ProgressTimerTick;
        }

        private void ProgressTimerTick(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(VideoLengthSeconds));
        }

        private void Pause()
        {
            mediaPlayer.Pause();
            IsPlaying = false;
            progressUpdateTimer.Stop();
        }

        private void Play()
        {
            mediaPlayer.Play();
            IsPlaying = true;
            progressUpdateTimer.Start();
        }
    }
}
