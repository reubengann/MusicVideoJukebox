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

        public PlaylistEditViewModelTest()
        {
            libraryStore = new LibraryStore();
            libraryStore.SetLibrary(1, "foobar");
            metadataManagerFactory = new FakeMetadataManagerFactory();
            dut = new PlaylistEditViewModel(libraryStore, metadataManagerFactory);
        }

        [Fact]
        public async Task LoadsPlaylists()
        {
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "playlist 1"});
            await dut.Initialize();
            Assert.Single(dut.Playlists);
        }

        [Fact]
        public async Task LoadsAvailableTracks()
        {
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "file1", Title = "title 1" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "file2", Title = "title 2" });
            await dut.Initialize();
            Assert.Equal(2, dut.AvailableTracks.Count);
        }

        [Fact]
        public async Task CannotSaveWithNoChanges()
        {
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "playlist 1" });
            await dut.Initialize();
            Assert.False(dut.SavePlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task IfNoLibrarySelectedDontCrash()
        {
            libraryStore.SetLibrary(null, null);
            await dut.Initialize();
        }

        [Fact]
        public async Task WhenAddingNewPlaylistDisablesBottomPanelUntilSaved()
        {
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
            await dut.Initialize();
            Assert.False(dut.DeletePlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task WhenAddingNewPlaylistCannotAddAnotherUntilSaved()
        {
            await dut.Initialize();
            dut.AddPlaylistCommand.Execute();
            Assert.False(dut.AddPlaylistCommand.CanExecute());
        }

        [Fact]
        public async Task WhenAddingNewPlaylistDontDuplicateName()
        {
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "New Playlist" });
            await dut.Initialize();
            dut.AddPlaylistCommand.Execute();
            Assert.Equal("New Playlist 2", dut.SelectedPlaylist?.Name);
        }

        [Fact]
        public async Task SavesAndRefreshesNewPlaylist()
        {
            await dut.Initialize();
            dut.AddPlaylistCommand.Execute();
            dut.SavePlaylistCommand.Execute();
            Assert.Equal(1, dut.SelectedPlaylist?.Id);
            Assert.Single(dut.Playlists);
            Assert.Equal(1, dut.Playlists.First().Id);
        }

        [Fact]
        public async Task SavesExistingPlaylist()
        {
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "New Playlist" });
            await dut.Initialize();
            dut.SelectedPlaylist = dut.Playlists[0];
            dut.SelectedPlaylist.Name = "changed";
            dut.SavePlaylistCommand.Execute();
            Assert.Equal("changed", metadataManagerFactory.ToReturn.Playlists[0].PlaylistName);
        }

        [Fact]
        public async Task LoadsExistingPlaylistTracksWhenInitializing()
        {
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "All Songs" });
            await dut.Initialize();
            Assert.NotNull(dut.SelectedPlaylist);
        }

        [Fact]
        public async Task WhenOnAllPlaylistCannotAddOrDelete()
        {
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { PlaylistId = 1, PlaylistName = "All Songs", IsAll = true });
            await dut.Initialize();
            dut.SelectedPlaylist = dut.Playlists.First();
            Assert.False(dut.AddTrackToPlaylistCommand.CanExecute());
            Assert.False(dut.DeleteTrackFromPlaylistCommand.CanExecute());
        }
    }
}
