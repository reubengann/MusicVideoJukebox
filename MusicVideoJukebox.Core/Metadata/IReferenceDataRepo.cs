
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IReferenceDataRepo
    {
        Task<List<FetchedMetadata>> GetCandidates(string artist, string track);
        Task<MetadataGetResult> TryGetExactMatch(string artist, string track);
    }
}
