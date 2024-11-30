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

        [Fact]
        public async Task CanGetBasicVideoInfo()
        {
            await dut.CreateTables();
            WithVideo("file1", "title1", "artist1", MetadataStatus.NotDone);
            WithVideo("file2", "title2", "artist2", MetadataStatus.NotDone);
            var result = await dut.GetAllMetadata();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CanUpdate()
        {
            await dut.CreateTables();
            var id = WithVideo("file1", "title1", "artist1", MetadataStatus.NotDone);
            await dut.UpdateMetadata(new VideoMetadata { VideoId = id, Artist = "artistupdated", Filename = "file1", Album = "album", Title = "titleupdated", ReleaseYear = 1984, Status = MetadataStatus.Manual });
            var conn = new SQLiteConnection(connectionString);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            var results = await conn.QueryAsync<VideoMetadata>("select * from video");
            Assert.Single(results);
            var result = results.First();
            Assert.Equal(MetadataStatus.Manual, result.Status);
            Assert.Equal("artistupdated", result.Artist);
            Assert.Equal("album", result.Album);
            Assert.Equal("titleupdated", result.Title);
            Assert.Equal(1984, result.ReleaseYear);
        }

        int WithVideo(string filename, string title, string artist, MetadataStatus status)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                var id = conn.ExecuteScalar<int>(@"INSERT INTO video (filename, title, artist, status) values (@filename, @title, @artist, @status) RETURNING video_id",
                    new { filename, title, artist, status });
                return id;
            }
        }
    }
}
