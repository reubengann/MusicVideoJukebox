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
        Task<GetAlbumYearResult> TryGetAlbumYear(string artist, string track);
        Task<bool> Resync();
        Task<List<PlaylistTrackForViewmodel>> GetPlaylistTracksForViewmodel(int playlistId);
        Task<List<PlaylistTrackForViewmodel>> ShuffleTracks(int playlistId);
        public Task<List<ScoredMetadata>> GetScoredCandidates(string artist, string track);

        IVideoRepo VideoRepo { get; }
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
