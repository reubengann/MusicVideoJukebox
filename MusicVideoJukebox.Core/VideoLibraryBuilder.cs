using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public static class VideoLibraryBuilder
    {
        public static async Task<VideoLibrary> BuildFromName(string folder, string playlistName)
        {
            var databaseFile = Path.Combine(folder, "meta.db");
            using var conn = new SQLiteConnection($"Data Source={databaseFile}");
            var videoRows = await conn.QueryAsync<VideoRow>(@"select A.* from
videos A
join playlists_videos B
ON A.video_id = B.video_id
join playlists C
ON B.playlist_id = C.playlist_id
where C.playlist_name = @PlaylistName
order by B.play_order", new { PlaylistName = playlistName });
            var fileNames = videoRows.Select(x => Path.Combine(folder, x.filename)).ToList();
            var infoMap = videoRows.ToDictionary(x => x.video_id, x => new VideoInfo { Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
            var playlistItemRows = await conn.QueryAsync<VideoIdPlaylistIdPair>("select playlist_id, video_id from playlists_videos order by playlist_id, play_order");
            var playlistIdToSongMap = new Dictionary<int, List<int>>();
            foreach (var row in playlistItemRows)
            {
                if (!playlistIdToSongMap.ContainsKey(row.playlist_id))
                    playlistIdToSongMap[row.playlist_id] = new List<int>();
                playlistIdToSongMap[row.playlist_id].Add(row.video_id);
            }
            var playlistRows = await conn.QueryAsync<PlaylistRow>("select playlist_id, playlist_name from playlists order by playlist_id");
            return new VideoLibrary(fileNames, infoMap, folder, playlistIdToSongMap, playlistRows.Select(x => new Playlist { PlaylistId = x.playlist_id, PlaylistName = x.playlist_name }).ToList());
        }
    }

    class VideoIdPlaylistIdPair
    {
        public int video_id { get; set; }
        public int playlist_id { get; set; }
    }
}
