using System;

namespace MusicVideoJukebox
{
    public interface IMediaPlayer
    {
        void Play();
        void Stop();
        void Pause();
        double LengthSeconds { get; }
        double CurrentTimeSeconds { get; set; }
        void SetSource(Uri source);
    }
}
