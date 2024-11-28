
using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core.Metadata
{
    public class VideoRepo : IVideoRepo
    {
        private readonly string connectionString;

        public VideoRepo(string folderPath)
        {
            var filepath = Path.Combine(folderPath, "meta.db");
            connectionString = $"Data Source={filepath};Pooling=False;";
        }

        public async Task CreateTables()
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync(@"
                create table if not exists video (
                video_id integer primary key autoincrement,
                filename text NOT NULL,
                ""year"" integer NULL,
                title text NOT NULL, 
                album text NULL, 
                artist text NOT NULL
                )
            ");
            await conn.ExecuteAsync(@"
                create table if not exists playlist (
                playlist_id integer primary key autoincrement,
                playlist_name text not null
                )
                ");
            await conn.ExecuteAsync(@"
                create table if not exists playlists_video (
                playlists_videos_id integer primary key autoincrement,
                playlist_id integer not null,
                video_id integer not null,
                play_order integer not null
                )
                ");
            await conn.ExecuteAsync(@"
                create table if not exists play_status (
                playlist_id integer null,
                song_id integer null
                )
                ");
        }
    }
}
