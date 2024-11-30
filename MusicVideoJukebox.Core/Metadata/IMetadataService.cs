namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataFetchResult
    {
        public bool Success { get; set; }
        public int? Year { get; set; }
        public string? AlbumTitle { get; set; }
    }

    public interface IMetadataService
    {
        Task<MetadataFetchResult> GetMetadata(string artist, string track);
    }
}
