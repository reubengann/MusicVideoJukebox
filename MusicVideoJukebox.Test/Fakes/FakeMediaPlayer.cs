using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMediaPlayer : IMediaPlayer
    {
        List<Uri> SourcesSet = [];
        public bool SetToPlay = false;

        double volume = 0;

        public double LengthSeconds => throw new NotImplementedException();

        public double CurrentTimeSeconds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Volume { get => volume; set => volume = value; }

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
            SetToPlay = true;
        }

        public void SetFullScreen()
        {
            throw new NotImplementedException();
        }

        public void SetSource(Uri source)
        {
            SourcesSet.Add(source);
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
