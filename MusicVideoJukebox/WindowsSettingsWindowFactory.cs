namespace MusicVideoJukebox
{
    class WindowsSettingsWindowFactory : ISettingsWindowFactory
    {
        public ISettingsWindow Create(VideoLibraryStore videoLibraryStore)
        {
            return new SettingsWindow(videoLibraryStore);
        }
    }
}
