
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IReferenceDataRepo
    {
        Task<List<SearchResult>> SearchReferenceDb(string artist, string title);
    }
}
