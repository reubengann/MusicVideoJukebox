using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public static class VideoLibraryBuilder
    {
        public static async Task<VideoLibrary> Build(string folder)
        {
            var databaseFile = Path.Combine(folder, "meta.db");
            using var destConnection = new SQLiteConnection($"Data Source={databaseFile}");
            var rows = await destConnection.QueryAsync<VideoRow>("SELECT video_id, filename, \"year\", title, album, artist FROM videos");
            var fileNames = rows.Select(x => Path.Combine(folder, x.filename)).ToList();
            var infoMap = rows.ToDictionary(x => Path.Combine(folder, x.filename), x => new VideoInfo { Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
            return new VideoLibrary(fileNames, infoMap);
        }
    }
}
