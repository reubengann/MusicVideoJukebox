using System;
using System.Numerics;
using System.Windows.Controls;
using MusicVideoJukebox.Core;

namespace MusicVideoJukebox
{
    public class MediaElementMediaPlayer : IMediaPlayer2
    {
        private readonly MediaElement media;

        public MediaElementMediaPlayer(MediaElement media)
        {
            this.media = media;
        }

        public void Pause()
        {
            media.Pause();
        }

        public void Play()
        {
            media.Play();
        }

        public double LengthSeconds
        {
            get
            {
                if (media.NaturalDuration.HasTimeSpan)
                    return media.NaturalDuration.TimeSpan.TotalSeconds;
                return 1;
            }
        }

        public double CurrentTimeSeconds
        {
            get => media.Position.TotalSeconds;
            set => media.Position = TimeSpan.FromSeconds(value);
        }
    }
}
