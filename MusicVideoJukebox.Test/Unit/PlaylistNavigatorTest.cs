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
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy", "track2.mp4", "title", 1);
            var output = await dut.Resume();
            Assert.Equal("track2.mp4", output?.FileName);
        }

        [Fact]
        public async Task OnInitializeAndNeverPlayedDefaultToFirstTrack()
        {
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = null;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy", "track2.mp4", "title", 1);
            var output = await dut.Resume();
            Assert.Equal("track1.mp4", output?.FileName);
        }

        [Fact]
        public async Task IfNoSongsAtAllResumeReturnNull()
        {
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = null;
            WithPlaylist(true, 1, "All");
            var output = await dut.Resume();
            Assert.Null(output);
        }

        [Fact]
        public async Task OnNextGoesOn()
        {
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
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
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 1;
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
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 1;
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
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 1;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            var foo = dut.Next() ?? throw new Exception();
            Assert.Equal(2, metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task OnPrevUpdatesStatus()
        {
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            var foo = dut.Previous() ?? throw new Exception();
            Assert.Equal(1, metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task IfCurrentTrackGetsRemovedFromPlaylistThenMustChange()
        {
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            DeleteTrack(2);
            var result = await dut.CheckPlaylistState();
            Assert.True(result.NeedsToChangeTrack);
            Assert.Equal(1, metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task IfCurrentTrackStillInPlaylistThenNoChange()
        {
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = 1;
            metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 2;
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("someguy", "track1.mp4", "title", 1);
            WithPlaylistTrack("someguy2", "track2.mp4", "title2", 2);
            await dut.Resume();
            DeleteTrack(1);
            var result = await dut.CheckPlaylistState();
            Assert.False(result.NeedsToChangeTrack);
            Assert.Equal(1, metadataManager.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder);
        }

        void WithMetadata(string filename)
        {
            metadataManager.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "", Filename = filename, Title = "" });
        }

        void WithPlaylist(bool isAll, int playlistId, string playlistName)
        {
            metadataManager.ConcreteVideoRepo.Playlists.Add(new Playlist { IsAll = isAll, PlaylistId = playlistId, PlaylistName = playlistName });
        }

        void WithPlaylistTrack(string artist, string filename, string title, int playOrder)
        {
            metadataManager.ConcreteVideoRepo.PlaylistTracks.Add(new PlaylistTrack { VideoId = playOrder, PlaylistId = playOrder, Artist = artist, FileName = filename, Title = title, PlayOrder = playOrder });
        }

        void DeleteTrack(int playOrder)
        {
            var item = metadataManager.ConcreteVideoRepo.PlaylistTracks.First(x => x.PlayOrder == playOrder);
            var idx = metadataManager.ConcreteVideoRepo.PlaylistTracks.IndexOf(item);
            for (int i = idx + 1; i < metadataManager.ConcreteVideoRepo.PlaylistTracks.Count; i++)
            {
                var old = metadataManager.ConcreteVideoRepo.PlaylistTracks[i];
                metadataManager.ConcreteVideoRepo.PlaylistTracks[i] = new PlaylistTrack
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
            metadataManager.ConcreteVideoRepo.PlaylistTracks.RemoveAt(idx);
        }
    }
}
