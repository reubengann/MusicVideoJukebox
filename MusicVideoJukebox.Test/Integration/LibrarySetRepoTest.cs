using Dapper;
using MusicVideoJukebox.Core.Libraries;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Test.Integration
{
    public class LibrarySetRepoTest : IDisposable
    {
        LibrarySetRepo dut;
        const string connectionString = @"Data Source=c:\repos\librarytest.db;Pooling=false;";

        public LibrarySetRepoTest()
        {
            using var conn = new SQLiteConnection(connectionString);
            dut = new LibrarySetRepo(connectionString);
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
            WithLibrary();
            var result = await dut.GetAllLibraries();
            Assert.Single(result);
            Assert.Equal(@"c:\foo\bar", result[0].FolderPath);
            Assert.Equal("libraryfoo", result[0].Name);
        }

        int WithLibrary()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                var id = conn.ExecuteScalar<int>(@"INSERT INTO library (folder_path, name) values ('c:\foo\bar', 'libraryfoo') RETURNING library_id");
                conn.Close();
                return id;
            }
        }
    }
}
