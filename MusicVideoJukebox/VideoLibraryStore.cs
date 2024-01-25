using MusicVideoJukebox.Core;

namespace MusicVideoJukebox
{
    public class VideoLibraryStore
    {
        private VideoLibrary videoLibrary;

        public VideoLibraryStore(VideoLibrary videoLibrary)
        {
            this.videoLibrary = videoLibrary;
        }

        public VideoLibrary VideoLibrary { get => videoLibrary; set { videoLibrary = value; } }
    }
}
