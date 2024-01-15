using System;

namespace MusicVideoJukebox
{
    public interface IMediaPlayer
    {
        void Play();
        void Stop();
        void Pause();
        double LengthSeconds { get; }
        double CurrentTime { get; }
        void SetSource(Uri source);
    }
}
