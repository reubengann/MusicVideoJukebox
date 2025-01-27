
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IReferenceDataRepo
    {
        Task<List<FetchedMetadata>> GetCandidates(string artist, string track, int searchLength = 3);
        Task<MetadataGetResult> TryGetExactMatch(string artist, string track);
    }
}
