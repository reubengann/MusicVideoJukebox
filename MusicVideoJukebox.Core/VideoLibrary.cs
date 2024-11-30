using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core
{
    //public class VideoLibrary
    //{
    //    public string Folder;
    //    public Dictionary<int, string> FilePaths;
    //    public Dictionary<int, VideoInfo> VideoIdToInfoMap;
    //    public Dictionary<int, List<int>> PlaylistIdToSongMap;
    //    public Dictionary<int, List<VideoInfoAndOrder>> PlaylistIdToSongOrderMap;
    //    public List<Playlist> Playlists;
    //    public List<VideoInfoWithId> VideoInfos;
    //    public ProgressPersister ProgressPersister;

    //    public VideoLibrary(Dictionary<int, string> filenames,
    //        Dictionary<int, VideoInfo> infoMap, string folder,
    //        Dictionary<int, List<int>> playlistIdToSongMap,
    //        List<Playlist> playlists,
    //        List<VideoInfoWithId> videoInfos,
    //        Dictionary<int, List<VideoInfoAndOrder>> playlistIdToSongOrderMap,
    //        ProgressPersister progressPersister)
    //    {
    //        FilePaths = filenames;
    //        VideoIdToInfoMap = infoMap;
    //        Folder = folder;
    //        PlaylistIdToSongMap = playlistIdToSongMap;
    //        Playlists = playlists;
    //        VideoInfos = videoInfos;
    //        PlaylistIdToSongOrderMap = playlistIdToSongOrderMap;
    //        ProgressPersister = progressPersister;
    //    }
    //}



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