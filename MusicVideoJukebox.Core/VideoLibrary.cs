namespace MusicVideoJukebox.Core
{
    public class VideoLibrary
    {
        public List<string> FilePaths;
        public Dictionary<string, VideoInfo> InfoMap;

        public VideoLibrary(List<string> filenames, Dictionary<string, VideoInfo> infoMap)
        {
            FilePaths = filenames;
            InfoMap = infoMap;
        }

        public static VideoLibrary FromFileList(List<string> filePaths)
        {
            var infoMap = new Dictionary<string, VideoInfo>();
            int ct = 0;
            foreach (string file in filePaths)
            {
                if (!file.EndsWith(".mp4")) continue;
                ct++;
                var nameOnly = Path.GetFileNameWithoutExtension(file);
                if (nameOnly.Contains(" - "))
                {
                    var parts = nameOnly.Split(" - ");
                    if (parts.Length > 2)
                    {
                        string.Join(" - ", parts.Skip(1));
                    }
                    infoMap[file] = new VideoInfo { Artist = parts[0], Title = parts[1] };
                }
                else
                {
                    infoMap[file] = new VideoInfo { Title = file };
                }
                if (infoMap.Count != ct)
                {
                    throw new System.Exception();
                }
            }
            return new VideoLibrary(filePaths, infoMap);
        }


    }

    public class VideoInfo
    {
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
                if (parts.Length > 2)
                {
                    string.Join(" - ", parts.Skip(1));
                }
                return (parts[0], parts[1]);
            }
            else
            {
                return ("Unknown", nameOnly);
            }
        }
    }
}