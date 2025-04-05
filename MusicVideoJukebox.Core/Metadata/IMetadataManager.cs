namespace MusicVideoJukebox.Core.Metadata
{
    public class SearchResult 
    {
        public string Title { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public string? AlbumTitle { get; set; }
        public int? ReleaseYear { get; set; }
    }

    

    public interface IMetadataManager
    {
        Task EnsureCreated();
        Task<bool> Resync();
        Task<List<PlaylistTrackForViewmodel>> GetPlaylistTracksForViewmodel(int playlistId);
        Task<List<PlaylistTrackForViewmodel>> ShuffleTracks(int playlistId);
        Task<List<SearchResult>> SearchReferenceDb(string query);

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
