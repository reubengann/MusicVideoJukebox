using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistEditViewModelTest
    {
        PlaylistEditViewModel dut;
        LibraryStore libraryStore;
        FakeMetadataManagerFactory metadataManagerFactory;
        FakeLibrarySetRepo librarySetRepo;
        FakeDialogService dialogService;
        FakeImageScalerService imageScalerService;

        public PlaylistEditViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            libraryStore = new LibraryStore(librarySetRepo);
            dialogService = new FakeDialogService();
            imageScalerService = new FakeImageScalerService();

            metadataManagerFactory = new FakeMetadataManagerFactory();
            dut = new PlaylistEditViewModel(libraryStore, metadataManagerFactory, dialogService, imageScalerService);
        }

        [Fact]
        public async Task LoadsPlaylists()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "playlist 1" });
            await dut.Initialize();
            Assert.Single(dut.Playlists);
        }

        [Fact]
        public async Task LoadsAvailableTracks()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "file1", Title = "title 1" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "file2", Title = "title 2" });
            await dut.Initialize();
            Assert.Equal(2, dut.AvailableTracks.Count);
        }

        [Fact]
        public async Task IfNoLibrarySelectedDontCrash()
        {
            await libraryStore.SetLibrary(null, null);
            await dut.Initialize();
        }

        [Fact]
        public async Task CannotDeleteWhenNothingSelected()
        {
            await libraryStore.SetLibrary(1, "foobar");
            await dut.Initialize();
            Assert.False(dut.DeletePlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task WhenAddingNewPlaylistDontDuplicateName()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "New Playlist" });
            await dut.Initialize();
            dialogService.AcceptedDetailsResult = true;
            dialogService.AcceptedDetailsObject.PlaylistName = "changed";
            dut.AddPlaylistCommand.Execute();
            Assert.Equal("New Playlist 2", dut.SelectedPlaylist?.Name);
        }

        [Fact]
        public async Task SavesAndRefreshesNewPlaylist()
        {
            await libraryStore.SetLibrary(1, "foobar");
            await dut.Initialize();
            dialogService.AcceptedDetailsResult = true;
            dialogService.AcceptedDetailsObject.PlaylistName = "changed";
            dut.AddPlaylistCommand.Execute();
            Assert.Equal(1, dut.SelectedPlaylist?.Id);
            Assert.Single(dut.Playlists);
            Assert.Equal(1, dut.Playlists.First().Id);
        }

        [Fact]
        public async Task SavesExistingPlaylist()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "New Playlist" });
            await dut.Initialize();
            dialogService.AcceptedDetailsResult = true;
            dialogService.AcceptedDetailsObject.PlaylistName = "changed";
            dut.EditPlaylistDetailsCommand.Execute();
            Assert.False(dut.ErrorOccurred);
            Assert.Equal("changed", metadataManagerFactory.ToReturn.Playlists[0].PlaylistName);
            Assert.Equal("changed", dut.SelectedPlaylist?.Name); // not really tested
        }

        [Fact]
        public async Task LoadsExistingPlaylistTracksWhenInitializing()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "All Songs" });
            await dut.Initialize();
            Assert.NotNull(dut.SelectedPlaylist);
        }

        [Fact]
        public async Task WhenOnAllPlaylistCannotAddOrDelete()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "All Songs", IsAll = true });
            await dut.Initialize();
            dut.SelectedPlaylist = dut.Playlists.First();
            Assert.False(dut.AddTrackToPlaylistCommand.CanExecute());
            Assert.False(dut.DeleteTrackFromPlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task WhenShufflingDoesIt()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "All Songs", IsAll = true });
            await dut.Initialize();
            dut.ShufflePlaylistCommand.Execute();
            Assert.True(metadataManagerFactory.ToReturn.WasShuffled);
        }

        [Fact]
        public async Task WhenFilteringShowCorrectTracks()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "All Songs", IsAll = true });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "file1", Title = "title 1" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "file2", Title = "title 2" });
            await dut.Initialize();
            Assert.Equal(2, dut.FilteredAvailableTracks.Count);
            dut.AvailableTracksFilter = "2";
            Assert.Single(dut.FilteredAvailableTracks);
        }

        [Fact]
        public async Task CanAddTracksToPlaylist()
        {
            await libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1, PlaylistName = "All Songs", IsAll = true });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "file1", Title = "title 1" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "file2", Title = "title 2" });
            await dut.Initialize();
            foreach (var t in dut.FilteredAvailableTracks)
            {
                t.IsSelected = true;
            }
            dut.AddTrackToPlaylistCommand.Execute();
            Assert.Equal(2, metadataManagerFactory.ToReturn.AddedToPlaylist.Count);
        }
    }
}
