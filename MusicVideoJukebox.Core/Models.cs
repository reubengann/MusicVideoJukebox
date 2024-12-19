namespace MusicVideoJukebox.Core
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = null!;
        public bool IsAll {  get; set; }
    }
}
