namespace MusicVideoJukebox.Core.Metadata
{
    public interface IVideoRepo
    {
        Task<bool> IsDatabaseInitialized();
        Task InitializeDatabase();
        Task AddBasicInfos(List<BasicInfo> basicInfos);
        Task<List<VideoMetadata>> GetAllMetadata();
        Task UpdateMetadata(VideoMetadata metadata);
        Task RemoveMetadata(int videoId);
        Task<List<Playlist>> GetPlaylists();
        Task<int> InsertPlaylist(Playlist playlist);
        Task UpdatePlaylistDetails(Playlist playlist);
        Task<int> AppendSongToPlaylist(int playlistId, int videoId);
        Task<int> GetTrackCountForPlaylist(int playlistId);
        Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order);
        Task UpdatePlaylistTrackOrderBatch(List<(int playlistId, int videoId, int order)> updates);
        Task DeleteFromPlaylist(int playlistId, int videoId);
        Task<List<PlaylistTrack>> GetPlaylistTracks(int playlistId);
        Task UpdateAnalysisVolume(int videoId, double? lufs);
        Task UpdatePlayStatus(int playlistId, int songOrder);
        Task UpdateActivePlaylist(int playlistId);
        Task<PlaylistStatus> GetActivePlaylist();
        Task<int> AddTag(string tagName);
        Task AddTagToVideo(int videoId, int tagId);
        Task RemoveTagFromVideo(int videoId, int tagId);
        Task<List<string>> GetTagsForVideo(int videoId);
    }

    public class VideoAnalysisEntry
    {
        public int VideoId { get; set; }
        public string Filename { get; set; } = null!;
        public string? VideoCodec { get; set; }
        public string? VideoResolution { get; set; }
        public string? AudioCodec { get; set; }
        public string? Warning { get; set; }
        public double? LUFS { get; set; }
    }

    public class PlaylistTrack
    {
        public int PlaylistVideoId { get; set; }
        public int PlaylistId { get; set; }
        public int VideoId { get; set; }
        public int PlayOrder { get; set; }
        required public string Artist { get; set; }
        required public string Title { get; set; }
        public string? Album { get; set; }
        public int? ReleaseYear { get; set; }
        required public string FileName { get; set; }
        public float LeadIn { get; set; } = 0;
        public float LeadOut { get; set; } = 0;
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
        public int? VideoHeight { get; set; }
        public int? VideoWidth { get; set; }
        public string? VideoCodec { get; set; }
        public string? AudioCodec { get; set; }
        public string? Warning { get; set; }
        public double? LUFS { get; set; }
        public double? LeadIn { get; set; } = 0;
        public double? LeadOut { get; set; } = 0;
    }
}
