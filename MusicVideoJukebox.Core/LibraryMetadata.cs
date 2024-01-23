namespace MusicVideoJukebox.Core
{
    public class LibraryMetadata
    {
        public string Folder { get; set; } = null!;
        public List<string> PlaylistNames { get; set; } = null!;
        public List<VideoInfo> VideoInfos { get; set; } = null!;
    }
}