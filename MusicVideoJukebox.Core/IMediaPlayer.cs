using System;

namespace MusicVideoJukebox.Core
{
    public interface IMediaPlayer
    {
        void Play();
        void Pause();
        double LengthSeconds { get; }
        double CurrentTimeSeconds { get; set; }
        void SetSource(Uri source);
        void Stop();
        void FadeInfoIn();
        void FadeInfoOut();
        void HideInfoImmediate();

        double Volume { get; set; }
    }
}
