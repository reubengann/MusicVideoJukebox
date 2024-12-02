using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class VideoPlayingViewModel : BaseViewModel
    {
        private bool isPlaying = false;
        private readonly IMediaPlayer2 mediaplayer;

        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }

        public VideoPlayingViewModel(IMediaPlayer2 mediaElementMediaPlayer)
        {
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            this.mediaplayer = mediaElementMediaPlayer;
        }

        private void Pause()
        {
            mediaplayer.Pause();
            IsPlaying = false;
        }

        private void Play()
        {
            mediaplayer.Play();
            IsPlaying = true;
        }
    }
}
