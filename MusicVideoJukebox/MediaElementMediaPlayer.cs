using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MusicVideoJukebox.Core;

namespace MusicVideoJukebox
{
    public class MediaElementMediaPlayer : IMediaPlayer2
    {
        private readonly MediaElement media;
        private readonly VideoInfoDisplay videoInfoDisplay;
        private readonly DependencyProperty opacityProperty;

        public MediaElementMediaPlayer(MediaElement media, VideoInfoDisplay videoInfoDisplay, DependencyProperty opacityProperty)
        {
            this.media = media;
            this.videoInfoDisplay = videoInfoDisplay;
            this.opacityProperty = opacityProperty;
        }

        public void SetSource(Uri source) => media.Source = source;

        public void Pause()
        {
            media.Pause();
        }

        public void Play()
        {
            media.Play();
        }

        public void Stop()
        {
            media.Stop();
        }

        public void FadeInfoIn()
        {
            videoInfoDisplay.BeginAnimation(opacityProperty, new DoubleAnimation { To = 1, Duration = TimeSpan.FromSeconds(0.25) });
        }

        public void FadeInfoOut()
        {
            videoInfoDisplay.BeginAnimation(opacityProperty, new DoubleAnimation { To = 0, Duration = TimeSpan.FromSeconds(0.25) });
        }

        public void HideInfoImmediate()
        {
            videoInfoDisplay.Opacity = 0;
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
        public double Volume { get => media.Volume; set => media.Volume = value; }
    }
}
