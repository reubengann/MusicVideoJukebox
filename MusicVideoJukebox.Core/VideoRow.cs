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
}
