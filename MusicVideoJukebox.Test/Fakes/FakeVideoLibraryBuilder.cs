using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeVideoLibraryBuilder : IVideoLibraryBuilder
    {
        public VideoLibrary ToReturn = new([], [], "", [], [], [], [], new ProgressPersister("", new CurrentPlayStatus()));

        public async Task<VideoLibrary> BuildAsync(string folder)
        {
            await Task.CompletedTask;
            return ToReturn;
        }
    }
}
