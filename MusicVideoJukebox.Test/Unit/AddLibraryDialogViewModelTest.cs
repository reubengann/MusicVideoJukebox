using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class AddLibraryDialogViewModelTest
    {
        AddLibraryDialogViewModel dut;
        FakeDialogService dialogService;
        FakeLibrarySetRepo librarySetRepo;

        public AddLibraryDialogViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            dialogService = new FakeDialogService();
            dut = new AddLibraryDialogViewModel(dialogService, librarySetRepo);
        }

        [Fact]
        public void WhenFolderIsFakeThenShowAnError()
        {
            dut.FolderPath = "somethingtotallyfake";
            dut.SaveCommand.Execute(null);
            Assert.True(dialogService.ShowedError);
        }

        [Fact]
        public void WhenFolderIsInDbRejectIt()
        {
            librarySetRepo.LibraryItems.Add(new LibraryItem { Name = "foo", FolderPath = @"c:\" });
            dut.FolderPath = @"c:\";
            dut.SaveCommand.Execute(null);
            Assert.True(dialogService.ShowedError);
        }

        [Fact]
        public void WhenNameIsInDbRejectIt()
        {
            librarySetRepo.LibraryItems.Add(new LibraryItem { Name = "foo", FolderPath = @"c:\" });
            dut.FolderPath = @"c:\Windows";
            dut.LibraryName = @"foo";
            dut.SaveCommand.Execute(null);
            Assert.True(dialogService.ShowedError);
        }

        [Fact]
        public void WhenValidReportGood()
        {
            bool requestedClose = false;
            dut.RequestClose += (s) => requestedClose = s;
            dut.FolderPath = @"c:\Windows";
            dut.LibraryName = @"foo";
            dut.SaveCommand.Execute(null);
            Assert.False(dialogService.ShowedError);
            Assert.True(requestedClose);
        }
    }
}
