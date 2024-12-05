namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataGetResult
    {
        public bool Success { get; set; }
        public FetchedMetadata? FetchedMetadata { get; set; }
    }

    public class FetchedMetadata
    {
        required public string Title { get; set; }
        required public string Artist { get; set; }
        required public int FirstReleaseDateYear { get; set; }
        required public string AlbumTitle { get; set; }
    }

    public interface IMetadataService
    {
        Task<MetadataGetResult> GetMetadata(string artist, string track);
    }
}
