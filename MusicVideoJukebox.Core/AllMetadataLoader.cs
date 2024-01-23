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
            var playlistRows = await conn.QueryAsync<PlaylistRow>("select playlist_id, playlist_name from playlists");
            var videoRows = await conn.QueryAsync<VideoRow>("SELECT video_id, filename, \"year\", title, album, artist FROM videos order by artist, title");
            var videoInfos = videoRows.Select(x => new VideoInfoWithId { VideoId = x.video_id, Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
            var playlists = playlistRows.Select(x => new Playlist { PlaylistId = x.playlist_id, PlaylistName = x.playlist_name });
            return new LibraryMetadata { Folder = folder, Playlists = playlists.ToList(), VideoInfos = videoInfos.ToList() };
        }
    }
}
