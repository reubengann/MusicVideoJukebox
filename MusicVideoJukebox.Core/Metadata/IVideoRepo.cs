
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IVideoRepo
    {
        Task CreateTables();
        Task AddBasicInfos(List<BasicInfo> basicInfos);
        Task<List<VideoMetadata>> GetAllMetadata();
        Task UpdateMetadata(VideoMetadata metadata);
        Task RemoveMetadata(int videoId);
        Task<List<Playlist>> GetPlaylists();
        Task<int> SavePlaylist(Playlist playlist);
        Task UpdatePlaylistName(int id, string name);
        Task<List<PlaylistTrackForViewmodel>> GetPlaylistTrackForViewmodels(int playlistId);
        Task AppendSongToPlaylist(int playlistId, int videoId);
        Task<int> GetTrackCountForPlaylist(int playlistId);
        Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order);
    }

    public class PlaylistTrackForViewmodel
    {
        public int PlaylistVideoId { get; set; }
        public int PlaylistId { get; set; }
        public int VideoId { get; set; }
        public int PlayOrder { get; set; }
        required public string Artist { get; set; }
        required public string Title { get; set; }
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
