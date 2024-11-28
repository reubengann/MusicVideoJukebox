
using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core.Libraries
{
    public class LibrarySetRepo : ILibrarySetRepo
    {
        private string connectionString;

        public LibrarySetRepo(string dbPath)
        {
            connectionString = $"Data Source={dbPath};Pooling=False;";
        }

        public async Task AddLibrary(LibraryItemAdd libraryItem)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("INSERT INTO library (folder_path, name) VALUES (@FolderPath, @Name)", libraryItem);
        }

        public async Task<List<LibraryItem>> GetAllLibraries()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using (var conn = new SQLiteConnection(connectionString))
            {
                var result = await conn.QueryAsync<LibraryItem>(@"
                    SELECT library_id, folder_path, name from library;");
                conn.Close();
                return result.ToList();
            }
        }

        public async Task<List<string>> GetAllLibraryNames()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                var result = await conn.QueryAsync<string>(@"
                    SELECT name from library;");
                conn.Close();
                return result.ToList();
            }
        }

        public async Task<List<string>> GetAllLibraryPaths()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                var result = await conn.QueryAsync<string>(@"
                    SELECT folder_path from library;");
                conn.Close();
                return result.ToList();
            }
        }

        public async Task Initialize()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                await conn.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS library (
                    library_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    folder_path TEXT NOT NULL,
                    name TEXT NOT NULL,
                    song_count INT NULL,
                    playlist_count INT NULL
                );");
                conn.Close();
            }
        }
    }
}
