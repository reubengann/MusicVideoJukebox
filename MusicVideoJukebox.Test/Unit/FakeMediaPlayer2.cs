using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeMediaPlayer2 : IMediaPlayer2
    {
        public bool IsPlaying { get; set; } = false;
        public double InternalPosition = 0;
        public double InternalLength = 0;
        public string FilePlaying = "";

        public double LengthSeconds => InternalLength;

        public double CurrentTimeSeconds { get => InternalPosition; set { InternalPosition = value; } }

        public double Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void SetSource(Uri source)
        {
            FilePlaying = source.OriginalString;
        }

        public void Stop()
        {
            IsPlaying = false;
        }
    }
}