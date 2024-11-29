
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IVideoRepo
    {
        Task CreateTables();
        Task AddBasicInfos(List<BasicInfo> basicInfos);
        Task<List<VideoMetadata>> GetAllMetadata();
    }

    public class BasicInfo
    {
        required public string Title { get; set; }
        required public string Artist { get; set; }
        required public string Filename { get; set; }
    }

    public enum MetadataStatus
    {
        NotDone = 0,
        Fetching = 1,
        Done = 2,
        Manual = 3,
        NotFound = 4
    }

    public class VideoMetadata
    {
        public int VideoId { get; set; }
        required public string Artist { get; set; }
        required public string Title { get; set; }
        required public string Filename { get; set; }
        public string? Album { get; set; }
        public int? ReleaseYear { get; set; }
        public MetadataStatus Status { get; set; }
    }
}
