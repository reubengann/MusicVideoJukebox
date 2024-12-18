using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistPlayViewModelTest
    {
        PlaylistPlayViewModel dut;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;

        public PlaylistPlayViewModelTest()
        {
            metadataManagerFactory = new FakeMetadataManagerFactory();
            libraryStore = new LibraryStore();
            libraryStore.SetLibrary(1, "foo");
            dut = new PlaylistPlayViewModel(libraryStore, metadataManagerFactory);
        }

        [Fact]
        public async Task OnInitializeLoadPlaylists()
        {
            metadataManagerFactory.ToReturn.Playlists.Add(new Core.Playlist { IsAll = true, PlaylistId = 1,  PlaylistName = "All songs" });
            await dut.Initialize();
            Assert.Single(dut.Items);
        }
    }
}
