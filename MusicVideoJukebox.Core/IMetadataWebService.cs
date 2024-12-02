using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core
{
    public interface IMetadataWebService
    {
        Task<List<MetadataCandidate>> GetMetadataCandidates(string artist, string track);
    }

    public class MetadataCandidate
    {
        required public int AlbumId { get; set; }
        required public string AlbumTitle { get; set; }
        required public string ArtistName { get; set; }
        required public string SongTitle { get; set; }
    }
}
