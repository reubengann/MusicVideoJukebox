using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class LibraryStoreTest
    {
        LibraryStore dut;
        FakeLibrarySetRepo librarySetRepo;
        FakeMetadataManagerFactory metadataManagerFactory;

        public LibraryStoreTest()
        {
            metadataManagerFactory = new FakeMetadataManagerFactory();
            librarySetRepo = new FakeLibrarySetRepo();
            dut = new LibraryStore(librarySetRepo, metadataManagerFactory);
        }

        [Fact]
        public async Task LoadsStateOnInitialize()
        {
            librarySetRepo.CurrentState.LibraryId = 1;
            await dut.Initialize();
            Assert.Equal(1, dut.CurrentState.LibraryId);
        }

        [Fact]
        public async Task CanUpdateLibraryId()
        {
            librarySetRepo.CurrentState.LibraryId = 1;
            await dut.Initialize();
            await dut.SetLibrary(3, "foo");
            Assert.Equal(3, librarySetRepo.CurrentState.LibraryId);
        }
    }
}
