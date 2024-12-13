
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

        public async Task AddBasicInfos(List<BasicInfo> basicInfos)
        {
            const string query = @"
        INSERT INTO video (filename, title, artist, status)
        VALUES (@Filename, @Title, @Artist, 0);";

            using var conn = new SQLiteConnection(connectionString);
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();

            await conn.ExecuteAsync(query, basicInfos, transaction);

            await transaction.CommitAsync();
        }

        public async Task CreateTables()
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync(@"
                create table if not exists video (
                video_id integer primary key autoincrement,
                filename text NOT NULL,
                release_year integer NULL,
                title text NOT NULL, 
                album text NULL, 
                artist text NOT NULL,
                status integer NOT NULL
                )
            ");
            await conn.ExecuteAsync(@"
                create table if not exists playlist (
                playlist_id integer primary key autoincrement,
                playlist_name text not null,
                is_all boolean not null
                )
                ");
            await conn.ExecuteAsync(@"
                create table if not exists playlist_video (
                playlists_videos_id integer primary key autoincrement,
                playlist_id integer not null,
                video_id integer not null,
                play_order integer not null,
                foreign key (playlist_id) references playlist (playlist_id) on delete cascade,
                foreign key (video_id) references video (video_id) on delete cascade
                )
                ");
            await conn.ExecuteAsync(@"
                create table if not exists play_status (
                playlist_id integer null,
                song_id integer null
                )
                ");
        }

        public async Task<List<VideoMetadata>> GetAllMetadata()
        {
            using var conn = new SQLiteConnection(connectionString);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            return (await conn.QueryAsync<VideoMetadata>("SELECT video_id, filename, release_year, title, album, artist, status from video")).ToList();
        }

        public async Task<List<Playlist>> GetPlaylists()
        {
            using var conn = new SQLiteConnection(connectionString);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            return (await conn.QueryAsync<Playlist>("SELECT playlist_id, playlist_name from playlist")).ToList();
        }

        public async Task RemoveMetadata(int videoId)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("DELETE FROM video WHERE video_id = @videoId;", new { videoId });
        }

        public async Task<int> SavePlaylist(Playlist playlist)
        {
            using var conn = new SQLiteConnection(connectionString);
            return await conn.ExecuteScalarAsync<int>("INSERT INTO playlist (playlist_name, is_all) VALUES (@playlistName, @isAll) RETURNING playlist_id", new { playlistName = playlist.PlaylistName, isAll = playlist.IsAll });
        }

        public async Task UpdateMetadata(VideoMetadata metadata)
        {
            using var conn = new SQLiteConnection(connectionString);
            const string query = @"
            UPDATE video
            SET title = @Title,
            artist = @Artist,
            album = @Album,
            release_year = @ReleaseYear,
            status = @Status
            WHERE video_id = @VideoId;";
            await conn.ExecuteAsync(query, metadata);
        }

        public async Task UpdatePlaylist(Playlist playlist)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("UPDATE playlist SET playlist_name = @playlistName WHERE playlist_id = @playlistId", new { playlistName  = playlist.PlaylistName, playlistId = playlist.PlaylistId });
        }
    }
}
