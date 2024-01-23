using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public static class MetadataLoader
    {
        public static async Task<LibraryMetadata> LoadAsync(string folder)
        {
            var databaseFile = Path.Combine(folder, "meta.db");
            using var conn = new SQLiteConnection($"Data Source={databaseFile}");
            var playlistNames = await conn.QueryAsync<string>("select playlist_name from playlists");
            var videoRows = await conn.QueryAsync<VideoRow>("SELECT video_id, filename, \"year\", title, album, artist FROM videos order by artist, title");
            var videoInfos = videoRows.Select(x => new VideoInfoWithId { VideoId = x.video_id, Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
            return new LibraryMetadata { Folder = folder, PlaylistNames = playlistNames.ToList(), VideoInfos = videoInfos.ToList() };
        }
    }
}
