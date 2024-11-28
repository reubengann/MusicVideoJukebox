using Dapper;
using MusicVideoJukebox.Core.Metadata;
using System.Data.SQLite;

namespace MusicVideoJukebox.Test.Integration
{
    public class VideoRepoTest : IDisposable
    {
        const string connectionString = @"Data Source=c:\repos\meta.db;Pooling=false;";
        VideoRepo dut;

        public VideoRepoTest()
        {
            using var conn = new SQLiteConnection(connectionString);
            dut = new VideoRepo(@"c:\repos");
        }

        public void Dispose()
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Execute("DELETE FROM video");
        }

        [Fact]
        public async Task CanCreateTables()
        {
            await dut.CreateTables();
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("SELECT * from video");
        }

        [Fact]
        public async Task CanAddBasicVideoInfo()
        {
            await dut.CreateTables();
            await dut.AddBasicInfos([
                new BasicInfo { Artist = "artist1", Filename = "path1", Title = "name1" },
                new BasicInfo { Artist = "artist2", Filename = "path2", Title = "name2" },
                ]);
            using var conn = new SQLiteConnection(connectionString);
            var rowcount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from video");
            Assert.Equal(2, rowcount);
        }
    }
}
