namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataGetResult
    {
        public bool Success { get; set; }
        public int? Year { get; set; }
        public string? AlbumTitle { get; set; }
    }

    public interface IMetadataService
    {
        Task<MetadataGetResult> GetMetadata(string artist, string track);
    }
}
