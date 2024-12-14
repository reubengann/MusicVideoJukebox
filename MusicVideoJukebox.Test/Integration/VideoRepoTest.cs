using Dapper;
using MusicVideoJukebox.Core;
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
            conn.Execute("DROP table video");
            conn.Execute("DROP table playlist");
            conn.Execute("DROP table playlist_video");
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
            WithVideo(1, "file1", "title1", "artist1", "album1", MetadataStatus.NotDone);
            WithVideo(2, "file2", "title2", "artist2", "album2", MetadataStatus.NotDone);
            var result = await dut.GetAllMetadata();
            Assert.Equal(2, result.Count);
            Assert.Equal("album1", result[0].Album);
        }

        [Fact]
        public async Task CanUpdateVideo()
        {
            await dut.CreateTables();
            var id = WithVideo(1, "file1", "title1", "artist1", "album", MetadataStatus.NotDone);
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

        [Fact]
        public async Task CanGetPlaylists()
        {
            await dut.CreateTables();
            WithPlaylist(2, "playlist1");
            var result = await dut.GetPlaylists();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CanSaveNewPlaylist()
        {
            await dut.CreateTables();
            var id = await dut.SavePlaylist(new Playlist { PlaylistId = -1, PlaylistName = "New Playlist" });
            using var conn = new SQLiteConnection(connectionString);
            var rows = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from playlist");
            Assert.Equal(2, rows);
            var name = await conn.ExecuteScalarAsync<string>("SELECT playlist_name from playlist where is_all = 0");
            Assert.Equal("New Playlist", name);
        }

        [Fact]
        public async Task CanUpdatePlaylist()
        {
            await dut.CreateTables();
            WithPlaylist(2, "playlist1");
            await dut.UpdatePlaylist(2, "newname" );
            using var conn = new SQLiteConnection(connectionString);
            var rows = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from playlist");
            Assert.Equal(2, rows);
            var name = await conn.ExecuteScalarAsync<string>("SELECT playlist_name from playlist WHERE is_all = 0");
            Assert.Equal("newname", name);
        }

        [Fact]
        public async Task CanRetrievePlaylistVideosForViewmodel()
        {
            await dut.CreateTables();
            WithVideo(1, "", "title 1", "artist 1", "album 1", MetadataStatus.Done);
            WithVideo(2, "", "title 2", "artist 2", "album 2", MetadataStatus.Done);
            WithVideo(3, "", "title 3", "artist 3", "album 3", MetadataStatus.Done);
            WithPlaylist(2, "All Songs");
            WithPlaylistVideo(1, 2, 1, 1);
            WithPlaylistVideo(2, 2, 2, 2);
            WithPlaylistVideo(3, 1, 1, 1); // should not be returned
            var result = await dut.GetPlaylistTrackForViewmodels(2);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CanRetrieveTrackCountForPlaylist()
        {
            await dut.CreateTables();
            WithVideo(1, "", "title 1", "artist 1", "album 1", MetadataStatus.Done);
            WithVideo(2, "", "title 2", "artist 2", "album 2", MetadataStatus.Done);
            WithVideo(3, "", "title 3", "artist 3", "album 3", MetadataStatus.Done);
            WithPlaylist(2, "All Songs");
            WithPlaylistVideo(1, 2, 1, 1);
            WithPlaylistVideo(2, 2, 2, 2);
            WithPlaylistVideo(3, 1, 1, 1); // should not be returned
            var result = await dut.GetTrackCountForPlaylist(2);
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CanAppendToPlaylist()
        {
            await dut.CreateTables();
            WithVideo(1, "", "title 1", "artist 1", "album 1", MetadataStatus.Done);
            WithVideo(2, "", "title 2", "artist 2", "album 2", MetadataStatus.Done);
            WithVideo(3, "", "title 3", "artist 3", "album 3", MetadataStatus.Done);
            WithPlaylist(2, "All Songs");
            await dut.AppendSongToPlaylist(2, 2);
            await dut.AppendSongToPlaylist(2, 3);
            using var conn = new SQLiteConnection(connectionString);
            var ids = await conn.QueryAsync<int>("SELECT video_id from playlist_video where playlist_id = 2");
            Assert.Equal(new List<int> { 2, 3 }.ToHashSet(), ids.ToHashSet());
        }

        int WithVideo(int videoId, string filename, string title, string artist, string album, MetadataStatus status)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                var id = conn.ExecuteScalar<int>(@"INSERT INTO video (video_id, filename, title, artist, status, album) values (@videoId, @filename, @title, @artist, @status, @album) RETURNING video_id",
                    new { videoId, filename, title, artist, status, album });
                return id;
            }
        }

        void WithPlaylist(int id, string name)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.ExecuteScalar<int>(@"INSERT INTO playlist (playlist_id, playlist_name, is_all) values (@id, @name, 0)",
                    new { id, name });
            }
        }

        void WithPlaylistVideo(int playlistVideoId, int playlistId, int videoId, int playOrder)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.ExecuteScalar<int>(@"INSERT INTO playlist_video (playlist_video_id, playlist_id, video_id, play_order) values (@playlistVideoId, @playlistId, @videoId, @playOrder)",
                    new { playlistVideoId, playlistId, videoId, playOrder });
            }
        }
    }
}
