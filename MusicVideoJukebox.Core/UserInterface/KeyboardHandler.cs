using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Core.UserInterface
{
    public class KeyboardHandler : IKeyboardHandler
    {
        private readonly PlayingViewModel videoPlayingViewModel;

        public KeyboardHandler(PlayingViewModel videoPlayingViewModel)
        {
            this.videoPlayingViewModel = videoPlayingViewModel;
        }

        public void NextTrackPressed()
        {
            videoPlayingViewModel.SkipNextCommand.Execute(null);
        }

        public void PlayPauseKeyPressed()
        {
            videoPlayingViewModel.PlayCommand.Execute(null);
        }

        public void PrevTrackPressed()
        {
            videoPlayingViewModel.SkipPreviousCommand.Execute(null);
        }
    }
}
