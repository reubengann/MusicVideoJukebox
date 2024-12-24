using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistSelectViewModelTest
    {
        PlaylistSelectViewModel dut;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;
        FakeLibrarySetRepo librarySetRepo;
        FakeNavigationService navigationService;

        public PlaylistSelectViewModelTest()
        {
            navigationService = new FakeNavigationService();
            librarySetRepo = new FakeLibrarySetRepo();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            libraryStore = new LibraryStore(librarySetRepo);
            
            dut = new PlaylistSelectViewModel(libraryStore, metadataManagerFactory, navigationService);
        }

        [Fact]
        public async Task OnInitializeLoadPlaylists()
        {
            await libraryStore.SetLibrary(1, "foo");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { IsAll = true, PlaylistId = 1,  PlaylistName = "All songs" });
            await dut.Initialize();
            Assert.Single(dut.Items);
        }

        [Fact]
        public async Task SetsPlaylistOnSelect()
        {
            await libraryStore.SetLibrary(1, "foo");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { IsAll = true, PlaylistId = 1, PlaylistName = "All songs" });
            var other = new Playlist { IsAll = false, PlaylistId = 2, PlaylistName = "Other" };
            metadataManagerFactory.ToReturn.Playlists.Add(other);
            await dut.Initialize();
            dut.SelectPlaylistCommand.Execute(new PlaylistViewModel(other));
            Assert.Equal(2, libraryStore.CurrentState.PlaylistId);
        }
    }
}
