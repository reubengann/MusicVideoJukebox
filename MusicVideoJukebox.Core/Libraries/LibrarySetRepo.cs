
using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core.Libraries
{
    public class LibrarySetRepo : ILibrarySetRepo
    {
        private string connectionString;

        public LibrarySetRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task Initialize()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                await conn.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS library (
                    library_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    folder_path TEXT NOT NULL,
                    song_count INT NULL,
                    playlist_count INT NULL
                );");
                conn.Close();
            }
        }
    }
}
