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
                    infoMap[file] = new VideoInfo { Artist = artist, Title = title };
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