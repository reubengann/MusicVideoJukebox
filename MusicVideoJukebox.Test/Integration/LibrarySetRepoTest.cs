using Dapper;
using MusicVideoJukebox.Core.Libraries;
using System.Data.SQLite;

namespace MusicVideoJukebox.Test.Integration
{
    public class LibrarySetRepoTest : IDisposable
    {
        LibrarySetRepo dut;
        const string connectionString = @"Data Source=c:\repos\librarytest.db;Pooling=false;";

        public LibrarySetRepoTest()
        {
            using var conn = new SQLiteConnection(connectionString);
            dut = new LibrarySetRepo(@"c:\repos\librarytest.db");
        }

        public void Dispose()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Execute(@"DELETE FROM library;");
                //conn.Execute(@"DELETE FROM workout_session;");
                //conn.Execute(@"DELETE FROM exercise;");
                //conn.Execute(@"DELETE FROM routine;");
                conn.Close();
            }
        }

        [Fact]
        public async Task CanInitializeDb()
        {
            await dut.Initialize();
            using (var conn = new SQLiteConnection(connectionString))
            {
                await conn.ExecuteAsync(@"SELECT * FROM library;");
                conn.Close();
            }
        }

        [Fact]
        public async Task CanGetLibraries()
        {
            await dut.Initialize();
            WithLibrary(@"c:\foo\bar", "libraryfoo");
            var result = await dut.GetAllLibraries();
            Assert.Single(result);
            Assert.Equal(@"c:\foo\bar", result[0].FolderPath);
            Assert.Equal("libraryfoo", result[0].Name);
        }

        [Fact]
        public async Task CanGetAllPaths()
        {
            await dut.Initialize();
            WithLibrary(@"c:\folder1", "folder1");
            WithLibrary(@"c:\folder2", "folder2");
            var result = await dut.GetAllLibraryPaths();
            Assert.Equal(2, result.Count);
            Assert.Equal(result, [@"c:\folder1", @"c:\folder2"]);
        }

        [Fact]
        public async Task CanGetAllLibraryNames()
        {
            await dut.Initialize();
            WithLibrary(@"c:\folder1", "folder1");
            WithLibrary(@"c:\folder2", "folder2");
            var result = await dut.GetAllLibraryNames();
            Assert.Equal(2, result.Count);
            Assert.Equal(result, ["folder1", "folder2"]);
        }

        [Fact]
        public async Task CanInsertNewLibrary()
        {
            await dut.Initialize();
            await dut.AddLibrary(new LibraryItemAdd { FolderPath = "foo", Name = "bar" });
            using var conn = new SQLiteConnection(connectionString);
            var result = (await conn.QueryAsync<string>("SELECT folder_path FROM library")).ToList();
            Assert.Single(result);
            Assert.Equal("foo", result[0]);
        }

        int WithLibrary(string path, string name)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                var id = conn.ExecuteScalar<int>(@"INSERT INTO library (folder_path, name) values (@path, @name) RETURNING library_id",
                    new { path, name });
                conn.Close();
                return id;
            }
        }
    }
}
