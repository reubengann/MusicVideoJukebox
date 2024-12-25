using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class NewNavigatorTest
    {
        NewNavigator dut;
        FakeMetadataManager metadataManager;

        public NewNavigatorTest()
        {
            metadataManager = new FakeMetadataManager("whatever");
            dut = new NewNavigator(metadataManager);
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
            metadataManager.PlaylistTracks.Add(new PlaylistTrack { Artist = artist, FileName = filename, Title = title, PlayOrder = playOrder });
        }
    }
}
