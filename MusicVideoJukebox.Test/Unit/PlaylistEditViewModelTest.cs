using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistEditViewModelTest
    {
        PlaylistEditViewModel dut;
        LibraryStore libraryStore;
        FakeMetadataManagerFactory metadataManagerFactory;

        public PlaylistEditViewModelTest()
        {
            libraryStore = new LibraryStore();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            dut = new PlaylistEditViewModel(libraryStore, metadataManagerFactory);
        }

        [Fact]
        public async Task LoadsPlaylists()
        {
            libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "playlist 1"});
            await dut.Initialize();
            Assert.Single(dut.Playlists);
        }

        [Fact]
        public async Task CannotSaveWithNoChanges()
        {
            libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "playlist 1" });
            await dut.Initialize();
            Assert.False(dut.SavePlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task IfNoLibrarySelectedDontCrash()
        {
            await dut.Initialize();
        }

        [Fact]
        public async Task WhenAddingNewPlaylistDisablesBottomPanelUntilSaved()
        {
            libraryStore.SetLibrary(1, "foobar");
            await dut.Initialize();
            dut.AddPlaylistCommand.Execute();
            Assert.NotNull(dut.SelectedPlaylist);
            Assert.Equal(-1, dut.SelectedPlaylist.Id);
            Assert.True(dut.SelectedPlaylist.IsModified);
            Assert.False(dut.CanEditTracks);
        }

        [Fact]
        public async Task CannotDeleteWhenNothingSelected()
        {
            libraryStore.SetLibrary(1, "foobar");
            await dut.Initialize();
            Assert.False(dut.DeletePlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task WhenAddingNewPlaylistCannotAddAnotherUntilSaved()
        {
            libraryStore.SetLibrary(1, "foobar");
            await dut.Initialize();
            dut.AddPlaylistCommand.Execute();
            Assert.False(dut.AddPlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task WhenAddingNewPlaylistDontDuplicateName()
        {
            libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "New Playlist" });
            await dut.Initialize();
            dut.AddPlaylistCommand.Execute();
            Assert.Equal("New Playlist 2", dut.SelectedPlaylist?.Name);
        }
    }
}
