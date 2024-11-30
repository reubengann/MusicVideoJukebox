using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataProvider : IMetadataService
    {
        public Task<MetadataFetchResult> GetMetadata(string artist, string track)
        {
            throw new NotImplementedException();
        }
    }
}