using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeVideoLibraryBuilder : IVideoLibraryBuilder
    {
        public Task<VideoLibrary> BuildAsync(string folder)
        {
            throw new NotImplementedException();
        }
    }
}
