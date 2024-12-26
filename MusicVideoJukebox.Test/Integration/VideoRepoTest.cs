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
            conn.Execute("DROP table video");
            conn.Execute("DROP table playlist");
            conn.Execute("DROP table playlist_video");
            conn.Execute("DROP table video_analysis");
            conn.Execute("DROP table playlist_status");
            conn.Execute("DROP table active_playlist");
        }

        [Fact]
        public async Task CanCreateTables()
        {
            await dut.InitializeDatabase();
            using var conn = new SQLiteConnection(connectionString);
            await conn.ExecuteAsync("SELECT * from video");
        }

        [Fact]
        public async Task WhenCreatingTablesCreatesAnAllSongsPlaylistAndStatus()
        {
            await dut.InitializeDatabase();
            using var conn = new SQLiteConnection(connectionString);
            var playlistCount = await conn.ExecuteScalarAsync<int>("SELECT count(*) from playlist");
            Assert.Equal(1, playlistCount);
            var statusCount = await conn.ExecuteScalarAsync<int>("SELECT count(*) from playlist_status");
            Assert.Equal(1, statusCount);
            var activePlaylistCount = await conn.ExecuteScalarAsync<int>("SELECT count(*) from active_playlist");
            Assert.Equal(1, activePlaylistCount);
        }

        [Fact]
        public async Task CanAddBasicVideoInfo()
        {
            await dut.InitializeDatabase();
            await dut.AddBasicInfos([
                new BasicInfo { Artist = "artist1", Filename = "path1", Title = "name1" },
                new BasicInfo { Artist = "artist2", Filename = "path2", Title = "name2" },
                ]);
            using var conn = new SQLiteConnection(connectionString);
            var videoRowsCt = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from video");
            Assert.Equal(2, videoRowsCt);
            var playlistRowsCt = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from playlist_video WHERE playlist_id = 1");
            Assert.Equal(2, playlistRowsCt);
        }

        [Fact]
        public async Task CanRemoveMetadata()
        {
            await dut.InitializeDatabase();
            await dut.AddBasicInfos([
                new BasicInfo { Artist = "artist1", Filename = "path1", Title = "name1" },
                new BasicInfo { Artist = "artist2", Filename = "path2", Title = "name2" },
                ]);
            var conn = new SQLiteConnection(connectionString);
            var id1 = await conn.ExecuteScalarAsync<int>("SELECT video_id from video where artist = 'artist1'");
            var id2 = await conn.ExecuteScalarAsync<int>("SELECT video_id from video where artist = 'artist2'");
            var order = await conn.ExecuteScalarAsync<int>("SELECT play_order from playlist_video where video_id = @id1", new { id1 });
            Assert.Equal(1, order);
            await dut.RemoveMetadata(id1);
            var rowCt = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from video where video_id = @id1", new { id1 });
            Assert.Equal(0, rowCt);
            var newOrder = await conn.ExecuteScalarAsync<int>("SELECT play_order from playlist_video where video_id = @id2", new { id2 });
            Assert.Equal(1, newOrder);
        }

        [Fact]
        public async Task CanGetBasicVideoInfo()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "file1", "title1", "artist1", "album1", MetadataStatus.NotDone);
            WithVideo(2, "file2", "title2", "artist2", "album2", MetadataStatus.NotDone);
            var result = await dut.GetAllMetadata();
            Assert.Equal(2, result.Count);
            Assert.Equal("album1", result[0].Album);
        }

        [Fact]
        public async Task CanUpdateVideo()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "file1", "title1", "artist1", "album", MetadataStatus.NotDone);
            await dut.UpdateMetadata(new VideoMetadata { VideoId = 1, Artist = "artistupdated", Filename = "file1", Album = "album", Title = "titleupdated", ReleaseYear = 1984, Status = MetadataStatus.Manual });
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
        public async Task CanChangeActivePlaylist()
        {
            await dut.InitializeDatabase();
            WithPlaylist(2, "whatever");
            await dut.UpdateActivePlaylist(2);
            var conn = new SQLiteConnection(connectionString);
            var activePlaylistId = await conn.ExecuteScalarAsync<int>("SELECT playlist_id from active_playlist");
            Assert.Equal(2, activePlaylistId);
        }

        [Fact]
        public async Task CanUpdatePlaylistStatus()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "file1", "title1", "artist1", "album", MetadataStatus.NotDone);
            WithVideo(2, "file1", "title1", "artist1", "album", MetadataStatus.NotDone);
            await dut.UpdatePlayStatus(1, 2);
            var conn = new SQLiteConnection(connectionString);
            var songOrder = await conn.ExecuteScalarAsync<int>("SELECT song_order from playlist_status where playlist_id = 1");
            Assert.Equal(2, songOrder);
        }

        [Fact]
        public async Task CanGetPlaylists()
        {
            await dut.InitializeDatabase();
            WithPlaylist(2, "playlist1", description: "foobar");
            var result = await dut.GetPlaylists();
            Assert.Equal(2, result.Count);
            Assert.Equal("foobar", result[1].Description);
        }

        [Fact]
        public async Task CanSaveNewPlaylist()
        {
            await dut.InitializeDatabase();
            var id = await dut.InsertPlaylist(new Playlist { PlaylistId = -1, PlaylistName = "New Playlist" });
            using var conn = new SQLiteConnection(connectionString);
            var rows = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from playlist");
            Assert.Equal(2, rows);
            var name = await conn.ExecuteScalarAsync<string>("SELECT playlist_name from playlist where is_all = 0");
            Assert.Equal("New Playlist", name);
        }

        [Fact]
        public async Task CanUpdatePlaylist()
        {
            await dut.InitializeDatabase();
            WithPlaylist(2, "playlist1");
            await dut.UpdatePlaylistDetails(new Playlist { PlaylistId = 2, PlaylistName = "newname", Description = "something" });
            using var conn = new SQLiteConnection(connectionString);
            var rows = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) from playlist");
            Assert.Equal(2, rows);
            var name = await conn.ExecuteScalarAsync<string>("SELECT playlist_name from playlist WHERE is_all = 0");
            Assert.Equal("newname", name);
            var description = await conn.ExecuteScalarAsync<string?>("SELECT description from playlist WHERE is_all = 0");
            Assert.NotNull(description);
        }

        [Fact]
        public async Task CanRetrieveTrackCountForPlaylist()
        {
            await dut.InitializeDatabase();
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
            await dut.InitializeDatabase();
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

        [Fact]
        public async Task CanModifyPlaylistOrder()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "", "title 1", "artist 1", "album 1", MetadataStatus.Done);
            WithVideo(2, "", "title 2", "artist 2", "album 2", MetadataStatus.Done);
            WithVideo(3, "", "title 3", "artist 3", "album 3", MetadataStatus.Done);
            await dut.AppendSongToPlaylist(1, 1);
            await dut.AppendSongToPlaylist(1, 2);
            await dut.AppendSongToPlaylist(1, 3);
            await dut.UpdatePlaylistTrackOrder(1, 1, 2);
            await dut.UpdatePlaylistTrackOrder(1, 2, 1);
            using var conn = new SQLiteConnection(connectionString);
            var ids = await conn.QueryAsync<int>("SELECT video_id from playlist_video where playlist_id = 1 order by play_order");
            Assert.Equal([2, 1, 3], ids);
        }

        [Fact]
        public async Task CanGetPlaylistTracks()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "", "title 1", "artist 1", "album 1", MetadataStatus.Done, year: 1962);
            WithVideo(2, "", "title 2", "artist 2", "album 2", MetadataStatus.Done);
            WithVideo(3, "video3.mp4", "title 3", "artist 3", "album 3", MetadataStatus.Done);
            await dut.AppendSongToPlaylist(1, 1);
            await dut.AppendSongToPlaylist(1, 2);
            await dut.AppendSongToPlaylist(1, 3);
            var tracks = await dut.GetPlaylistTracks(1);
            Assert.Equal(3, tracks.Count);
            Assert.Equal("album 1", tracks[0].Album);
            Assert.Equal(1, tracks[0].PlaylistId);
            Assert.Equal(1962, tracks[0].ReleaseYear);
            Assert.Equal("artist 2", tracks[1].Artist);
            Assert.Equal(2, tracks[1].VideoId);
            Assert.Equal(1, tracks[1].PlaylistId);
            Assert.Equal("video3.mp4", tracks[2].FileName);
        }

        [Fact]
        public async Task CanInsertAnalysisResult()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "", "title 1", "artist 1", "album 1", MetadataStatus.Done, year: 1962);
            await dut.InsertAnalysisResult(new VideoAnalysisEntry { AudioCodec = "aac", LUFS = -23, VideoCodec = "avi", VideoId = 1, VideoResolution = "640x480", Warning = null });
            using var conn = new SQLiteConnection(connectionString);
            var rows = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM video_analysis");
            Assert.Equal(1, rows);
            var codec = await conn.ExecuteScalarAsync<string>("SELECT audio_codec FROM video_analysis");
            Assert.Equal("aac", codec);
        }

        [Fact]
        public async Task CanGetAnalysisResults()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "file1.mp4", "title 1", "artist 1", "album 1", MetadataStatus.Done, year: 1962);
            WithVideoAnalysis(1, "avi", "640x480", "aac", null, -23);
            var results = await dut.GetAnalysisResults();
            Assert.Single(results);
            Assert.Equal(1, results[0].VideoId);
            Assert.Equal("aac", results[0].AudioCodec);
            Assert.Equal("file1.mp4", results[0].Filename);
        }

        [Fact]
        public async Task CanUpdateLUFS()
        {
            await dut.InitializeDatabase();
            WithVideo(1, "file1.mp4", "title 1", "artist 1", "album 1", MetadataStatus.Done, year: 1962);
            WithVideoAnalysis(1, "avi", "640x480", "aac", null, -20);
            await dut.UpdateAnalysisVolume(1, -23);
            using var conn = new SQLiteConnection(connectionString);
            var lufs = await conn.ExecuteScalarAsync<double>("SELECT lufs FROM video_analysis WHERE video_id = 1");
            Assert.Equal(-23.0, lufs, 0.01);
        }

        [Fact]
        public async Task CanGetActivePlaylistInitial()
        {
            await dut.InitializeDatabase();
            var activeId = await dut.GetActivePlaylist();
            Assert.Equal(1, activeId.PlaylistId);
            Assert.Null(activeId.SongOrder);
        }

        [Fact]
        public async Task CanUpdateCurrentSongOrder()
        {
            await dut.InitializeDatabase();
            await dut.UpdateCurrentSongOrder(5);
            using var conn = new SQLiteConnection(connectionString);
            var songOrder = await conn.ExecuteScalarAsync<int>("SELECT song_order from playlist_status");
            Assert.Equal(5, songOrder);
        }

        void WithVideoAnalysis(int videoId, string videoCodec, string videoResolution, string audioCodec, string? warning, double? lufs)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Execute(
                @"INSERT INTO video_analysis (video_id, video_codec, video_resolution, audio_codec, warning, lufs) 
          VALUES (@videoId, @videoCodec, @videoResolution, @audioCodec, @warning, @lufs)",
                new { videoId, videoCodec, videoResolution, audioCodec, warning, lufs });
        }

        void WithVideo(int videoId, string filename, string title, string artist, string album, MetadataStatus status, int? year = null)
        {
            using var conn = new SQLiteConnection(connectionString);
            var id = conn.ExecuteScalar<int>(@"INSERT INTO video (video_id, filename, title, artist, status, album, release_year) values (@videoId, @filename, @title, @artist, @status, @album, @year)",
                new { videoId, filename, title, artist, status, album, year });
        }

        void WithPlaylist(int id, string name, string? description = null, string? imagePath = null)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.ExecuteScalar<int>(@"INSERT INTO playlist (playlist_id, playlist_name, description, image_path, is_all) values (@id, @name, @description, @imagePath, 0)",
                    new { id, name, description, imagePath });
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
