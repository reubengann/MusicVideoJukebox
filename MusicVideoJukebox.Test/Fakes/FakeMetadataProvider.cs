using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataProvider : IMetadataProvider
    {
        public Task<VideoInfo> GetMetadata(string artist, string track)
        {
            throw new NotImplementedException();
        }
    }
}