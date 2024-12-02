using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeMediaPlayer2 : IMediaPlayer2
    {
        public bool IsPlaying { get; set; } = false;

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Play()
        {
            IsPlaying = true;
        }
    }
}