using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeReferenceDataRepo : IReferenceDataRepo
    {
        public List<FetchedMetadata> ExactMatches = [];
        public List<FetchedMetadata> NearMatches = [];

        public Task<List<FetchedMetadata>> GetCandidates(string artist, string track, int searchLength = 3)
        {
            return Task.FromResult(NearMatches);
        }

        public async Task<MetadataGetResult> TryGetExactMatch(string artist, string track)
        {
            await Task.CompletedTask;
            if (ExactMatches.Any())
            {
                return new MetadataGetResult { Success = true, FetchedMetadata = ExactMatches[0] };
            }
            return new MetadataGetResult { Success = false };
        }
    }
}