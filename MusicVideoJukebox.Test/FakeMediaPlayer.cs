using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeMediaPlayer : IMediaPlayer
    {
        List<Uri> SourcesSet = [];

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
