namespace MusicVideoJukebox.Core
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = null!;
    }

    public class VideoInfo
    {
        public string Artist { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Album { get; set; }
        public int? Year { get; set; }
    }

    public class VideoInfoWithId
    {
        public int VideoId { get; set; }
        public string Artist { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Album { get; set; }
        public int? Year { get; set; }
    }
}
