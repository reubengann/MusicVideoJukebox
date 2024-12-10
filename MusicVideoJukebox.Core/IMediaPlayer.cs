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
        void SetWindowed();
        void SetFullScreen();
        void FadeButtonsOut();
        void MaybeFadeButtonsIn();
        void ShowInfo();
        void HideInfo();

        double Volume { get; set; }
    }

    public interface IMediaPlayer2
    {
        void Play();
        void Pause();
        double LengthSeconds { get; }
        double CurrentTimeSeconds { get; set; }
        void SetSource(Uri source);
        //void SetWindowed();
        //void SetFullScreen();
        //void FadeButtonsOut();
        //void MaybeFadeButtonsIn();
        //void ShowInfo();
        //void HideInfo();

        //double Volume { get; set; }
    }
}
