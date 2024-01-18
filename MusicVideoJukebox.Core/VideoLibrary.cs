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

        public class VideoInfo
        {
            public string? Artist { get; set; }
            public string Title { get; set; } = null!;
            public string? Album { get; set; }
            public string? Year { get; set; }
        }
    }
}