using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistPlayViewModelTest
    {
        PlaylistPlayViewModel dut;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;
        FakeLibrarySetRepo librarySetRepo;

        public PlaylistPlayViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            libraryStore = new LibraryStore(librarySetRepo);
            
            dut = new PlaylistPlayViewModel(libraryStore, metadataManagerFactory);
        }

        [Fact]
        public async Task OnInitializeLoadPlaylists()
        {
            await libraryStore.SetLibrary(1, "foo");
            metadataManagerFactory.ToReturn.Playlists.Add(new Playlist { IsAll = true, PlaylistId = 1,  PlaylistName = "All songs" });
            await dut.Initialize();
            Assert.Single(dut.Items);
        }
    }
}
