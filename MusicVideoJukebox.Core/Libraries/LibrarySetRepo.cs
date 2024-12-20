
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

        public async Task<CurrentState> GetCurrentState()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using var conn = new SQLiteConnection(connectionString);
            return (await conn.QueryAsync<CurrentState>("SELECT library_id, library_path, playlist_id, video_id, volume FROM app_state")).First();
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
                await conn.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS app_state (
                    library_id INTEGER NULL,
                    library_path TEXT NULL,
                    playlist_id INTEGER NULL,
                    video_id INTEGER NULL,
                    volume INTEGER NULL
                );");
                await conn.ExecuteAsync("INSERT INTO app_state (library_id) VALUES (null)");
                conn.Close();
            }
        }

        public async Task UpdateState(CurrentState currentState)
        {
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync(@"UPDATE app_state SET 
  library_id = @LibraryId
, library_path = @LibraryPath
, playlist_id = @PlaylistId
, video_id = @VideoId
, volume = @Volume", currentState);
        }
    }
}
