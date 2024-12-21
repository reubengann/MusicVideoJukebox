using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Test.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Test.Unit
{
    public class LibraryStoreTest
    {
        LibraryStore dut;
        FakeLibrarySetRepo librarySetRepo;

        public LibraryStoreTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            dut = new LibraryStore(librarySetRepo);
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
