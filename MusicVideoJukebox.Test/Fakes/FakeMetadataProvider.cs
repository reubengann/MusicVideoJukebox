using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataProvider : IMetadataWebService
    {
        public Task<List<MetadataCandidate>> GetMetadataCandidates(string artist, string track)
        {
            throw new NotImplementedException();
        }
    }
}