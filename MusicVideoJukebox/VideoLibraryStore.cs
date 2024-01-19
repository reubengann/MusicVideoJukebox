using MusicVideoJukebox.Core;

namespace MusicVideoJukebox
{
    public class VideoLibraryStore
    {
        private readonly VideoLibrary videoLibrary;

        public VideoLibraryStore(VideoLibrary videoLibrary)
        {
            this.videoLibrary = videoLibrary;
        }

        public VideoLibrary VideoLibrary => videoLibrary;
    }
}
