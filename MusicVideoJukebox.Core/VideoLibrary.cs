namespace MusicVideoJukebox.Core
{
    public class VideoLibrary
    {
        public string Folder;
        public List<string> FilePaths;
        public Dictionary<string, VideoInfo> InfoMap;

        public VideoLibrary(List<string> filenames, Dictionary<string, VideoInfo> infoMap, string folder)
        {
            FilePaths = filenames;
            InfoMap = infoMap;
            Folder = folder;
        }
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

    public static class FileNameHelpers
    {
        public static (string, string) ParseFileNameIntoArtistTitle(string filename)
        {
            var nameOnly = Path.GetFileNameWithoutExtension(filename);
            if (nameOnly.Contains(" - "))
            {
                var parts = nameOnly.Split(" - ");
                string title;
                if (parts.Length > 2)
                {
                    title = string.Join(" - ", parts.Skip(1));
                }
                else
                {
                    title = parts[1];
                }
                string artist = parts[0];
                return (artist, title);
            }
            else
            {
                return ("Unknown", nameOnly);
            }
        }
    }
}