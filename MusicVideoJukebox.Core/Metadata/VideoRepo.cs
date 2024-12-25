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
        VALUES (@Filename, @Title, @Artist, 0) RETURNING video_id;";

            using var conn = new SQLiteConnection(connectionString);
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();

            foreach (var basicInfo in basicInfos)
            {
                var videoId = await conn.ExecuteScalarAsync<int>(query, basicInfo, transaction);
                await conn.ExecuteAsync(
                @"INSERT INTO playlist_video (playlist_id, video_id, play_order)
          VALUES (
              1,
              @videoId,
              COALESCE(
                  (SELECT MAX(play_order) + 1 FROM playlist_video WHERE playlist_id = 1),
                  1
              )
          )",
                new { videoId });
            }

            await transaction.CommitAsync();
        }

        public async Task<int> AppendSongToPlaylist(int playlistId, int videoId)
        {
            using var conn = new SQLiteConnection(connectionString);

            var id = await conn.ExecuteScalarAsync<int>(
                @"INSERT INTO playlist_video (playlist_id, video_id, play_order)
          VALUES (
              @playlistId,
              @videoId,
              COALESCE(
                  (SELECT MAX(play_order) + 1 FROM playlist_video WHERE playlist_id = @playlistId),
                  1
              )
          )
            RETURNING playlist_video_id",
                new { playlistId, videoId });
            return id;
        }

        public async Task InitializeDatabase()
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
                description TEXT,
                image_path TEXT,
                is_all boolean not null
                )
                ");
            await conn.ExecuteAsync(@"
                create table if not exists playlist_video (
                playlist_video_id integer primary key autoincrement,
                playlist_id integer not null,
                video_id integer not null,
                play_order integer not null,
                foreign key (playlist_id) references playlist (playlist_id) on delete cascade,
                foreign key (video_id) references video (video_id) on delete cascade
                )
                ");
            await conn.ExecuteAsync(@"
                create table if not exists playlist_status (
                playlist_id integer not null,
                song_order integer null
                )
                ");
            await conn.ExecuteAsync(@"
                create table if not exists active_playlist (
                playlist_id integer not null
                )
                ");
            await conn.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS video_analysis (
                    video_analysis_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    video_id INTEGER NOT NULL,
                    video_codec TEXT,
                    video_resolution TEXT,
                    audio_codec TEXT,
                    warning TEXT,
                    lufs REAL,
                    FOREIGN KEY (video_id) REFERENCES video(video_id) ON DELETE CASCADE
                );
            ");
            await SetupTasks(conn);
        }

        async Task SetupTasks(SQLiteConnection conn)
        {
            // should be 1 ...
            var allSongPlaylistId = await InsertPlaylist(conn, new Playlist { PlaylistName = "All songs", Description = "Every song in the library", IsAll = true });
            await conn.ExecuteAsync(@"INSERT INTO playlist_status (playlist_id) VALUES (@allSongPlaylistId)", new { allSongPlaylistId });
            await conn.ExecuteAsync(@"INSERT INTO active_playlist (playlist_id) VALUES (@allSongPlaylistId)", new { allSongPlaylistId });
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
            return (await conn.QueryAsync<Playlist>("SELECT playlist_id, playlist_name, description, image_path, is_all from playlist")).ToList();
        }

        public async Task<int> GetTrackCountForPlaylist(int playlistId)
        {
            using var conn = new SQLiteConnection(connectionString);
            return await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM playlist_video WHERE playlist_id = @playlistId", new { playlistId });
        }

        public async Task<int> InsertPlaylist(Playlist playlist)
        {
            using var conn = new SQLiteConnection(connectionString);
            return await InsertPlaylist(conn, playlist);
        }

        async Task<int> InsertPlaylist(SQLiteConnection conn, Playlist playlist)
        {
            var createdId = await conn.ExecuteScalarAsync<int>(@"
INSERT INTO playlist (playlist_name, description, image_path, is_all) 
VALUES (@PlaylistName, @Description, @ImagePath, @IsAll) RETURNING playlist_id", playlist);
            // Also need to make sure we have a playlist_status

            return createdId;
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

        public async Task RemoveMetadata(int videoId)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();
            try
            {
                // Fetch playlists containing the video
                var playlistsContainingVideo = await conn.QueryAsync<int>(
                    @"SELECT playlist_id FROM playlist_video WHERE video_id = @videoId;",
                    new { videoId }, transaction);

                // Remove the video from each playlist and update play orders
                foreach (var playlistId in playlistsContainingVideo)
                {
                    await DeleteFromPlaylist(conn, playlistId, videoId, transaction);
                }

                // Remove the video from the video table
                await conn.ExecuteAsync(
                    "DELETE FROM video WHERE video_id = @videoId;",
                    new { videoId }, transaction);

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback the transaction in case of any errors
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteFromPlaylist(int playlistId, int videoId)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();
            try
            {
                await DeleteFromPlaylist(conn, playlistId, videoId, transaction);
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback(); throw;
            }
        }

        static async Task DeleteFromPlaylist(SQLiteConnection conn, int playlistId, int videoId, SQLiteTransaction transaction)
        {
            var playOrders = (await conn.QueryAsync<int>(
                @"SELECT play_order 
          FROM playlist_video 
          WHERE playlist_id = @playlistId AND video_id = @videoId
          ORDER BY play_order;",
                new { playlistId, videoId }, transaction)).ToList();

            if (playOrders.Count == 0) return; // If no matching entries, exit early

            foreach (var playOrder in playOrders)
            {
                // Remove the specific occurrence of the video
                await conn.ExecuteAsync(
                    @"DELETE FROM playlist_video 
              WHERE playlist_id = @playlistId AND video_id = @videoId AND play_order = @playOrder;",
                    new { playlistId, videoId, playOrder }, transaction);

                // Adjust the play order for the remaining videos after the deleted one
                await conn.ExecuteAsync(
                    @"UPDATE playlist_video
              SET play_order = play_order - 1
              WHERE playlist_id = @playlistId AND play_order > @playOrder;",
                    new { playlistId, playOrder }, transaction);
            }
        }

        public async Task UpdatePlaylistDetails(Playlist playlist)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync(
                @"
UPDATE playlist 
SET playlist_name = @name,
description = @description,
image_path = @imagePath
WHERE playlist_id = @id
", new { id = playlist.PlaylistId, name = playlist.PlaylistName, description = playlist.Description, imagePath = playlist.ImagePath });
        }

        public async Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("UPDATE playlist_video SET play_order = @order WHERE video_id = @videoId and playlist_id = @playlistId", new { playlistId, videoId, order });
        }

        public async Task<List<PlaylistTrack>> GetPlaylistTracks(int playlistId)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using var conn = new SQLiteConnection(connectionString);
            var rows = await conn.QueryAsync<PlaylistTrack>(@"
                SELECT playlist_video_id, playlist_id, A.video_id, play_order, artist, B.title, B.album, B.release_year, B.filename
                FROM playlist_video A
                JOIN video B
                on A.video_id = B.video_id
                WHERE playlist_id = @playlistId
                ORDER by A.play_order
                ", new { playlistId });
            return rows.ToList();
        }

        public async Task InsertAnalysisResult(VideoAnalysisEntry entry)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync(
                @"INSERT INTO video_analysis (video_id, video_codec, video_resolution, audio_codec, warning, lufs) 
          VALUES (@VideoId, @VideoCodec, @VideoResolution, @AudioCodec, @Warning, @LUFS)",
                entry);
        }

        public async Task<List<VideoAnalysisEntry>> GetAnalysisResults()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using var conn = new SQLiteConnection(connectionString);
            return (await conn.QueryAsync<VideoAnalysisEntry>(
                @"SELECT filename, A.video_id, video_codec, video_resolution, 
                 audio_codec, warning, lufs
                FROM video_analysis A
                join video B on A.video_id = B.video_id
                ")).ToList();
        }

        public async Task UpdateAnalysisVolume(int videoId, double? lufs)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("UPDATE video_analysis SET lufs = @lufs WHERE video_id = @videoId", new { videoId, lufs });
        }

        public async Task UpdatePlayStatus(int playlistId, int songOrder)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("UPDATE playlist_status SET song_order = @songOrder WHERE playlist_id = @playlistId", new { playlistId, songOrder });
        }

        public async Task UpdateActivePlaylist(int playlistId)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("UPDATE active_playlist SET playlist_id = @playlistId", new { playlistId });
        }
    }
}
