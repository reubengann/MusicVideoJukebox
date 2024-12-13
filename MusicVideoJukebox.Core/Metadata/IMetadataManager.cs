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
        Task UpdatePlaylist(int id, string name);
        Task<bool> Resync();
    }
}
