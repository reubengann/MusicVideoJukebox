using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlayingViewModelTest
    {
        PlayingViewModel dut;
        FakeMediaPlayer2 mediaPlayer2;
        FakeUIThreadFactory threadFactory;
        FakeUiThreadTimer progressBarTimer;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;
        FakeInterfaceFader fader;
        FakeLibrarySetRepo librarySetRepo;

        public PlayingViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            fader = new FakeInterfaceFader();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            libraryStore = new LibraryStore(librarySetRepo, metadataManagerFactory);
            threadFactory = new FakeUIThreadFactory();
            progressBarTimer = new FakeUiThreadTimer();
            threadFactory.ToReturn.Add(progressBarTimer);
            threadFactory.ToReturn.Add(new FakeUiThreadTimer());
            mediaPlayer2 = new FakeMediaPlayer2();
            dut = new PlayingViewModel(mediaPlayer2, threadFactory, metadataManagerFactory, fader, libraryStore, null);
        }

        [Fact]
        public void WhenPausedNotPlaying()
        {
            mediaPlayer2.IsPlaying = true;
            dut.PauseCommand.Execute(null);
            Assert.False(dut.IsPlaying);
            Assert.False(mediaPlayer2.IsPlaying);
        }

        [Fact]
        public void WhenPlayedIsPlaying()
        {
            dut.PlayCommand.Execute(null);
            Assert.True(dut.IsPlaying);
            Assert.True(mediaPlayer2.IsPlaying);
            Assert.True(progressBarTimer.Started);
        }

        [Fact]
        public void EverySoOftenCheckTheMediaPlayer()
        {
            mediaPlayer2.InternalLength = 25;
            mediaPlayer2.InternalPosition = 5;
            progressBarTimer.Trigger();
            Assert.Equal(25, dut.VideoLengthSeconds);
            Assert.Equal(5, dut.VideoPositionTime);
        }

        [Fact]
        public async Task CheckTheLibraryStoreForNewPlaylistWhenTriggered()
        {
            WithMetadata("fake.mp4");
            WithPlaylist(true, 1, "All");
            WithPlaylist(false, 2, "Other");
            WithPlaylistTrack("", "c:\\afile.mp4", "", 1);
            await libraryStore.SetLibrary(1, "something");
            SetPlaylist(1);
            await dut.Recheck();
            SetPlaylist(2);
            await dut.Recheck();
            Assert.Equal(2, metadataManagerFactory.ToReturn.ConcreteVideoRepo.LastPlaylistQueried);
        }

        void SetPlaylist(int playlistId)
        {
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.CurrentActivePlaylistStatus.PlaylistId = playlistId;
            libraryStore.CurrentPlaylistId = playlistId;
        }


        [Fact]
        public async Task AdvancesTheTrack()
        {
            WithMetadata("fake.mp4");
            WithMetadata("fake2.mp4");
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("", "c:\\afile.mp4", "", 1);
            WithPlaylistTrack("", "c:\\afile2.mp4", "", 2);
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder = 1;
            await libraryStore.SetLibrary(1, "something");
            await dut.Recheck();
            Assert.Equal("c:\\afile.mp4", dut.CurrentPlaylistTrack?.FileName);
            dut.SkipNextCommand.Execute(null);
            Assert.Equal("c:\\afile2.mp4", dut.CurrentPlaylistTrack?.FileName);
            Assert.Equal(2, metadataManagerFactory.ToReturn.ConcreteVideoRepo.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task LoopsAroundWhenAtBeginning()
        {
            WithMetadata("fake.mp4");
            WithMetadata("fake2.mp4");
            WithPlaylist(true, 1, "All");
            WithPlaylistTrack("", "c:\\afile.mp4", "", 1);
            WithPlaylistTrack("", "c:\\afile2.mp4", "", 2);
            await libraryStore.SetLibrary(1, "something");
            await dut.Recheck();
            Assert.Equal("c:\\afile.mp4", dut.CurrentPlaylistTrack?.FileName);
            dut.SkipPreviousCommand.Execute(null);
            Assert.Equal("c:\\afile2.mp4", dut.CurrentPlaylistTrack?.FileName);
        }

        void WithMetadata(string filename)
        {
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "", Filename = filename, Title = "" });
        }

        void WithPlaylist(bool isAll, int playlistId, string playlistName)
        {
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.Playlists.Add(new Playlist { IsAll = isAll, PlaylistId = playlistId, PlaylistName = playlistName });
        }

        void WithPlaylistTrack(string artist, string filename, string title, int playOrder)
        {
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.PlaylistTracks.Add(new PlaylistTrack { Artist = artist, FileName = filename, Title = title, PlayOrder = playOrder });
        }
    }
}
