using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeMediaPlayer : IMediaPlayer
    {
        public double LengthSeconds => throw new NotImplementedException();

        public double CurrentTimeSeconds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void FadeButtonsOut()
        {
            throw new NotImplementedException();
        }

        public void HideInfo()
        {
            throw new NotImplementedException();
        }

        public void MaybeFadeButtonsIn()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void SetFullScreen()
        {
            throw new NotImplementedException();
        }

        public void SetSource(Uri source)
        {
            throw new NotImplementedException();
        }

        public void SetWindowed()
        {
            throw new NotImplementedException();
        }

        public void ShowInfo()
        {
            throw new NotImplementedException();
        }
    }
}
