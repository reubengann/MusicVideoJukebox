using System;
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
    }
}
