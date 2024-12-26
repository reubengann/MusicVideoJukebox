namespace MusicVideoJukebox.Core
{
    public class CurrentState
    {
        public int? LibraryId { get; set; }
        public string? LibraryPath { get; set; }
        public int? VideoId { get; set; }
        public int? Volume { get; set; }
    }
}
