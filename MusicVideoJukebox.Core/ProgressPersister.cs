using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public class ProgressPersister
    {
        private readonly string databaseFile;
        public CurrentPlayStatus CurrentPlayStatus;

        public ProgressPersister(string databaseFile, CurrentPlayStatus status)
        {
            this.databaseFile = databaseFile;
            this.CurrentPlayStatus = status;
        }

        public static async Task<ProgressPersister> CreateAsync(string databaseFile)
        {
            using var conn = new SQLiteConnection($"Data Source={databaseFile}");
            var status = await conn.QuerySingleAsync<CurrentPlayStatus>("select playlist_id, song_id from play_status");
            return new ProgressPersister(databaseFile, status);
        }

        public async Task StoreStatusAsync(int playlistId, int songId)
        {
            CurrentPlayStatus.playlist_id = playlistId;
            CurrentPlayStatus.song_id = songId;
            using var conn = new SQLiteConnection($"Data Source={databaseFile}");
            await conn.ExecuteAsync("update play_status set playlist_id = @PlaylistId, song_id = @SongId", new { PlaylistId = playlistId, SongId = songId });
        }
    }

    public class CurrentPlayStatus
    {
        public int playlist_id { get; set; }
        public int song_id { get; set; }
    }
}
