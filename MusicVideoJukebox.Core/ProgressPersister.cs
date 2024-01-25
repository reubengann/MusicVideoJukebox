using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public class ProgressPersister
    {
        private readonly string databaseFile;
        public CurrentPlayStatus CurrentPlayStatus;

        private ProgressPersister(string databaseFile, CurrentPlayStatus status)
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
    }

    public class CurrentPlayStatus
    {
        public int playlist_id { get; set; }
        public int song_id { get; set; }
    }
}
