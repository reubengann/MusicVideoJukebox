namespace MusicVideoJukebox.Core
{
    public class LibraryMetadata
    {
        public string Folder { get; set; } = null!;
        public List<string> PlaylistNames { get; set; } = null!;
        public List<VideoInfoWithId> VideoInfos { get; set; } = null!;
    }
}