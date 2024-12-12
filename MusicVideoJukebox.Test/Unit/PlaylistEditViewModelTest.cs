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
        public async Task IfNoLibrarySelectedDontCrash()
        {
            await dut.Initialize();
        }
    }
}
