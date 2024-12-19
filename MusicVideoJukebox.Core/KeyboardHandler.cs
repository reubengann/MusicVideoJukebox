using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Core
{
    public class KeyboardHandler : IKeyboardHandler
    {
        private readonly MainWindowViewModel newMainWindowViewModel;
        private readonly VideoPlayingViewModel videoPlayingViewModel;

        public KeyboardHandler(MainWindowViewModel newMainWindowViewModel, VideoPlayingViewModel videoPlayingViewModel)
        {
            this.newMainWindowViewModel = newMainWindowViewModel;
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
