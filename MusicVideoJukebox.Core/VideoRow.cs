namespace MusicVideoJukebox.Core
{
    public class VideoRow
    {
        public int video_id { get; set; }
        public string filename { get; set; } = null!;
        public int? year { get; set; }
        public string title { get; set; } = null!;
        public string? album { get; set; }
        public string artist { get; set; } = null!;
    }

    public class PlaylistsVideosRow
    {
        public int playlists_videos_id { get; set; }
        public int playlist_id { get; set; }
        public int video_id { get; set; }
        public int play_order { get; set; }
    }

    public class PlaylistRow
    {
        public int playlist_id { get; set; }
        public string playlist_name { get; set; } = null!;
    }
}
