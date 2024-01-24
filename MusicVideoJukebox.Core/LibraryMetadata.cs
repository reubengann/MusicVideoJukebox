namespace MusicVideoJukebox.Core
{
    public class LibraryMetadata
    {
        public string Folder { get; set; } = null!;
        public List<Playlist> Playlists { get; set; } = null!;
        public List<VideoInfoWithId> VideoInfos { get; set; } = null!;
        public Dictionary<int, List<VideoInfoAndOrder>> PlaylistMap = null!;
    }

    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = null!;
    }
}