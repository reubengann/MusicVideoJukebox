using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MusicVideoJukebox.Core;
using MusicVideoJukebox.Views;

namespace MusicVideoJukebox
{
    public class MediaElementMediaPlayer(MediaElement media, VideoInfoDisplay videoInfoDisplay, DependencyProperty opacityProperty) : IMediaPlayer
    {
        private readonly MediaElement media = media;
        private readonly VideoInfoDisplay videoInfoDisplay = videoInfoDisplay;
        private readonly DependencyProperty opacityProperty = opacityProperty;

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
            videoInfoDisplay.BeginAnimation(opacityProperty, new DoubleAnimation { To = 0, Duration = TimeSpan.FromSeconds(0.001) });
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
