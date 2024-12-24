namespace MusicVideoJukebox.Core.Metadata
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? ImagePath { get; set; } = null!;
        public bool IsAll { get; set; }
    }
}
