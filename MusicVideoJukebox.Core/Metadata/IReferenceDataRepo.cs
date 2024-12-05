namespace MusicVideoJukebox.Core.Metadata
{
    public interface IReferenceDataRepo
    {
        Task<MetadataGetResult> TryGetExactMatch(string artist, string track);
    }
}
