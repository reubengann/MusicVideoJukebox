using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public static class AllSongsVideoLibraryBuilder
    {
        public static async Task<VideoLibrary> Build(string folder)
        {
            var databaseFile = Path.Combine(folder, "meta.db");
            using var conn = new SQLiteConnection($"Data Source={databaseFile}");
            var rows = await conn.QueryAsync<VideoRow>("SELECT video_id, filename, \"year\", title, album, artist FROM videos order by artist, title");
            var fileNames = rows.Select(x => Path.Combine(folder, x.filename)).ToList();
            var infoMap = rows.ToDictionary(x => Path.Combine(folder, x.filename), x => new VideoInfo { Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
            return new VideoLibrary(fileNames, infoMap);
        }
    }

    public static class AllSongsShuffledVideoLibraryBuilder
    {
        public static async Task<VideoLibrary> Build(string folder)
        {
            var databaseFile = Path.Combine(folder, "meta.db");
            using var conn = new SQLiteConnection($"Data Source={databaseFile}");
            var rows = await conn.QueryAsync<VideoRow>(@"select A.* from
videos A
join playlists_videos B
ON A.video_id = B.video_id
join playlists C
ON B.playlist_id = C.playlist_id
where C.playlist_name = 'All Songs Shuffled'
order by B.play_order");
            var fileNames = rows.Select(x => Path.Combine(folder, x.filename)).ToList();
            var infoMap = rows.ToDictionary(x => Path.Combine(folder, x.filename), x => new VideoInfo { Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
            return new VideoLibrary(fileNames, infoMap);
        }
    }
}
