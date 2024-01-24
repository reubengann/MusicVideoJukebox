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
            var playlistVideos = await conn.QueryAsync<PlaylistsVideosRow>("select playlists_videos_id, playlist_id, video_id, play_order from playlists_videos");
            var videoInfos = videoRows.Select(x => new VideoInfoWithId { VideoId = x.video_id, Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
            var playlists = playlistRows.Select(x => new Playlist { PlaylistId = x.playlist_id, PlaylistName = x.playlist_name });
            var playlistMap = new Dictionary<int, List<VideoInfoAndOrder>>();
            var infoLookup = videoInfos.ToDictionary(x => x.VideoId);
            foreach (var playlist in playlists)
            {
                playlistMap[playlist.PlaylistId] = new List<VideoInfoAndOrder>();
            }
            foreach (var x in playlistVideos)
            {
                playlistMap[x.playlist_id].Add(new VideoInfoAndOrder { Info = infoLookup[x.video_id], PlayOrder = x.play_order });
            }
            return new LibraryMetadata { Folder = folder, Playlists = playlists.ToList(), VideoInfos = videoInfos.ToList(), PlaylistMap = playlistMap };
        }
    }

    public class VideoInfoAndOrder
    {
        public int PlayOrder { get; set; }
        public VideoInfoWithId Info { get; set; } = null!;
    }

    public class MetadataUpdater : IDisposable
    {
        SQLiteConnection conn;

        public MetadataUpdater(string folder)
        {
            var databaseFile = Path.Combine(folder, "meta.db");
            conn = new SQLiteConnection($"Data Source={databaseFile}");
        }

        public async Task<Playlist> AddNewPlaylist(string playlistName)
        {
            var id = await conn.ExecuteScalarAsync<int>("insert into playlists (playlist_name) values (@PlaylistName) returning playlist_id", new { PlaylistName = playlistName });
            return new Playlist { PlaylistId = id, PlaylistName = playlistName };
        }

        public void Dispose()
        {
            conn.Dispose();
        }

        public async Task UpdatePlaylistName(int playlistId, string newName)
        {
            await conn.ExecuteAsync("update playlists set playlist_name = @NewName where playlist_id = @PlaylistId", new { NewName = newName, PlaylistId = playlistId });
        }
    }
}
