using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeSettingsWindowFactory : ISettingsWindowFactory
    {
        public ISettingsWindow Create(VideoLibraryStore videoLibraryStore)
        {
            throw new NotImplementedException();
        }
    }
}
