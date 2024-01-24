namespace MusicVideoJukebox.Core
{
    public class VideoLibrary
    {
        public string Folder;
        public List<string> FilePaths;
        public Dictionary<int, VideoInfo> VideoIdToInfoMap;
        public Dictionary<int, List<int>> PlaylistIdToSongMap;
        public List<Playlist> Playlists;

        public VideoLibrary(List<string> filenames, Dictionary<int, VideoInfo> infoMap, string folder, Dictionary<int, List<int>> playlistIdToSongMap, List<Playlist> playlists)
        {
            FilePaths = filenames;
            VideoIdToInfoMap = infoMap;
            Folder = folder;
            PlaylistIdToSongMap = playlistIdToSongMap;
            Playlists = playlists;
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