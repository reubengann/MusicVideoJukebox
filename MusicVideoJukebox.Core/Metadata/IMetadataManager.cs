namespace MusicVideoJukebox.Core.Metadata
{
    public class GetAlbumYearResult 
    { 
        public bool Success { get; set; }
        public string? AlbumTitle { get; set; }
        public int? ReleaseYear { get; set; }
    }

    

    public interface IMetadataManager
    {
        Task EnsureCreated();
        Task<List<VideoMetadata>> GetAllMetadata();
        Task<GetAlbumYearResult> TryGetAlbumYear(string artist, string track);
        Task UpdateVideoMetadata(VideoMetadata entry);
        Task<List<Playlist>> GetPlaylists();
        Task<int> SavePlaylist(Playlist playlist);
        Task UpdatePlaylist(Playlist playlist);
        Task<bool> Resync();
        Task<List<PlaylistTrackForViewmodel>> GetPlaylistTracksForViewmodel(int playlistId);
        Task<List<PlaylistTrackForViewmodel>> ShuffleTracks(int playlistId);
        Task<int> AppendSongToPlaylist(int playlistId, int videoId);
        Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order);
        Task<List<PlaylistTrack>> GetPlaylistTracks(int playlistId);
        public Task<List<ScoredMetadata>> GetScoredCandidates(string artist, string track);
        Task InsertAnalysisResult(VideoAnalysisEntry entry);
        Task<List<VideoAnalysisEntry>> GetAnalysisResults();
        Task UpdateAnalysisVolume(int videoId, double? lufs);
        Task<PlaylistStatus> GetActivePlaylist();
        Task UpdateActivePlaylist(int playlistId);
        Task UpdatePlayStatus(int playlistId, int songOrder);
        Task UpdateAnalysisResult(VideoAnalysisEntry entry);
    }

    public class PlaylistStatus
    {
        public int PlaylistId { get; set; }
        public int? SongOrder { get; set; }
    }

    public class ScoredMetadata
    {
        required public string TrackName { get; set; }
        required public string ArtistName { get; set; }
        required public int? FirstReleaseDateYear { get; set; }
        required public string? AlbumTitle { get; set; }
        required public int Similarity { get; set; }
    }
}
