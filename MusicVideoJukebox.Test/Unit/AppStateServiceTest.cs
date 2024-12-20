using MusicVideoJukebox.Core;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class AppStateServiceTest
    {
        AppStateService dut;
        FakeLibrarySetRepo librarySetRepo;

        public AppStateServiceTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            dut = new AppStateService(librarySetRepo);
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
            await dut.UpdateLibraryId(3);
            Assert.Equal(3, librarySetRepo.CurrentState.LibraryId);
        }
    }
}
