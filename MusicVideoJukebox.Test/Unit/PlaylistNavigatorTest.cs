using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistNavigatorTest
    {
        PlaylistNavigator dut;
        FakeMetadataManager metadataManager;

        public PlaylistNavigatorTest()
        {
            metadataManager = new FakeMetadataManager("whatever");
            dut = new PlaylistNavigator(metadataManager);
        }

        [Fact]
        public async Task OnInitializeResumesIfPossible()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy", "track2.mp4", "title", 1);
            var output = await dut.Resume();
            Assert.Equal("track2.mp4", output?.FileName);
        }

        [Fact]
        public async Task OnInitializeAndNeverPlayedDefaultToFirstTrack()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = null;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy", "track2.mp4", "title", 1);
            var output = await dut.Resume();
            Assert.Equal("track1.mp4", output?.FileName);
        }

        [Fact]
        public async Task IfNoSongsAtAllResumeReturnNull()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = null;
            WithPlaylist(true, 1, "All");
            var output = await dut.Resume();
            Assert.Null(output);
        }

        [Fact]
        public async Task OnNextGoesOn()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            var output = dut.Next();
            Assert.Equal("track2.mp4", output?.FileName);
        }

        [Fact]
        public async Task PreviousWrapsAround()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = 1;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            var foo = dut.Previous() ?? throw new Exception();
            Assert.Equal("someguy2", foo.Artist);
        }

        [Fact]
        public async Task NextWrapsAround()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = 1;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            var foo = dut.Next() ?? throw new Exception();
            Assert.Equal("someguy2", foo.Artist);
        }

        [Fact]
        public async Task OnNextUpdatesStatus()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = 1;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            var foo = dut.Next() ?? throw new Exception();
            Assert.Equal(2, metadataManager.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task OnPrevUpdatesStatus()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            var foo = dut.Previous() ?? throw new Exception();
            Assert.Equal(1, metadataManager.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task IfCurrentTrackGetsRemovedFromPlaylistThenMustChange()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            DeleteTrack(2);
            var result = await dut.CheckPlaylistState();
            Assert.True(result.NeedsToChangeTrack);
            Assert.Equal(1, metadataManager.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task IfCurrentTrackStillInPlaylistThenNoChange()
        {
            metadataManager.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            DeleteTrack(1);
            var result = await dut.CheckPlaylistState();
            Assert.False(result.NeedsToChangeTrack);
            Assert.Equal(1, metadataManager.CurrentActivePlaylistStatus.SongOrder);
        }

        void WithMetadata(string filename)
        {
            metadataManager.MetadataEntries.Add(new VideoMetadata { Artist = "", Filename = filename, Title = "" });
        }

        void WithPlaylist(bool isAll, int playlistId, string playlistName)
        {
            metadataManager.Playlists.Add(new Playlist { IsAll = isAll, PlaylistId = playlistId, PlaylistName = playlistName });
        }

        void WithPlaylistTrack(string artist, string filename, string title, int playOrder)
        {
            metadataManager.PlaylistTracks.Add(new PlaylistTrack { VideoId = playOrder, PlaylistId = playOrder, Artist = artist, FileName = filename, Title = title, PlayOrder = playOrder });
        }

        void DeleteTrack(int playOrder)
        {
            var item = metadataManager.PlaylistTracks.First(x => x.PlayOrder == playOrder);
            var idx = metadataManager.PlaylistTracks.IndexOf(item);
            for (int i = idx + 1; i < metadataManager.PlaylistTracks.Count; i++)
            {
                var old = metadataManager.PlaylistTracks[i];
                metadataManager.PlaylistTracks[i] = new PlaylistTrack
                {
                    PlayOrder = old.PlayOrder - 1,
                    Artist = old.Artist,
                    VideoId = old.VideoId,
                    PlaylistId = old.PlaylistId,
                    FileName = old.FileName,
                    Title = old.Title,
                    PlaylistVideoId = old.PlaylistVideoId,
                };
            }
            metadataManager.PlaylistTracks.RemoveAt(idx);
        }
    }
}
