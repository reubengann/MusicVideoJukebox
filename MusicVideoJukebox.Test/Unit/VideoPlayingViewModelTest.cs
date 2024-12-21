using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class VideoPlayingViewModelTest
    {
        VideoPlayingViewModel dut;
        FakeMediaPlayer2 mediaPlayer2;
        FakeUIThreadFactory threadFactory;
        FakeUiThreadTimer progressBarTimer;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;
        FakeInterfaceFader fader;
        FakeLibrarySetRepo librarySetRepo;

        public VideoPlayingViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            fader = new FakeInterfaceFader();
            libraryStore = new LibraryStore(librarySetRepo);
            metadataManagerFactory = new FakeMetadataManagerFactory();  
            threadFactory = new FakeUIThreadFactory();
            progressBarTimer = new FakeUiThreadTimer();
            threadFactory.ToReturn.Add(progressBarTimer);
            threadFactory.ToReturn.Add(new FakeUiThreadTimer());
            mediaPlayer2 = new FakeMediaPlayer2();
            dut = new VideoPlayingViewModel(mediaPlayer2, threadFactory, metadataManagerFactory, fader, libraryStore);
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
        public async Task ChecksTheLibraryStoreWhenTriggered()
        {
            Assert.False(dut.IsPlaying);
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new Core.Metadata.VideoMetadata { Artist = "", Filename = "fake.mp4", Title = "" });
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { IsAll = true, PlaylistId = 1, PlaylistName = "All" });
            metadataManagerFactory.ToReturn.PlaylistTracks.Add(new PlaylistTrack { Artist = "", FileName = "c:\\afile.mp4", Title = "" });
            await libraryStore.SetLibrary(1, "something");
            await dut.Recheck();
            Assert.True(dut.IsPlaying);
        }

        [Fact]
        public async Task AdvancesTheTrack()
        {
            Assert.False(dut.IsPlaying);
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new Core.Metadata.VideoMetadata { Artist = "", Filename = "fake.mp4", Title = "" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new Core.Metadata.VideoMetadata { Artist = "", Filename = "fake2.mp4", Title = "" });
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { IsAll = true, PlaylistId = 1, PlaylistName = "All" });
            metadataManagerFactory.ToReturn.PlaylistTracks.Add(new PlaylistTrack { Artist = "", FileName = "c:\\afile.mp4", Title = "" });
            metadataManagerFactory.ToReturn.PlaylistTracks.Add(new PlaylistTrack { Artist = "", FileName = "c:\\afile2.mp4", Title = "" });
            await libraryStore.SetLibrary(1, "something");
            await dut.Recheck();
            Assert.Equal("c:\\afile.mp4", dut.CurrentPlaylistTrack?.FileName);
            dut.SkipNextCommand.Execute(null);
            Assert.Equal("c:\\afile2.mp4", dut.CurrentPlaylistTrack?.FileName);
        }

        [Fact]
        public async Task ReadvancesTheTrack()
        {
            Assert.False(dut.IsPlaying);
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new Core.Metadata.VideoMetadata { Artist = "", Filename = "fake.mp4", Title = "" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new Core.Metadata.VideoMetadata { Artist = "", Filename = "fake2.mp4", Title = "" });
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { IsAll = true, PlaylistId = 1, PlaylistName = "All" });
            metadataManagerFactory.ToReturn.PlaylistTracks.Add(new PlaylistTrack { Artist = "", FileName = "c:\\afile.mp4", Title = "" });
            metadataManagerFactory.ToReturn.PlaylistTracks.Add(new PlaylistTrack { Artist = "", FileName = "c:\\afile2.mp4", Title = "" });
            await libraryStore.SetLibrary(1, "something");
            await dut.Recheck();
            Assert.Equal("c:\\afile.mp4", dut.CurrentPlaylistTrack?.FileName);
            dut.SkipPreviousCommand.Execute(null);
            Assert.Equal("c:\\afile2.mp4", dut.CurrentPlaylistTrack?.FileName);
        }
    }
}
