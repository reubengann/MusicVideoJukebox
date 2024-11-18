using MusicVideoJukebox.Core;
using System.CodeDom;

namespace MusicVideoJukebox
{
    class WindowsSettingsWindowFactory : ISettingsWindowFactory
    {
        private readonly IVideoLibraryBuilder videoLibraryBuilder;

        public WindowsSettingsWindowFactory(IVideoLibraryBuilder videoLibraryBuilder)
        {
            this.videoLibraryBuilder = videoLibraryBuilder;
        }

        public ISettingsWindow Create(VideoLibraryStore videoLibraryStore)
        {
            return new SettingsWindow(videoLibraryStore, videoLibraryBuilder);
        }
    }
}
