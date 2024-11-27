using MusicVideoJukebox.Core;
using MusicVideoJukebox.Test.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Test.Unit
{
    public class LibraryViewModelTest
    {
        LibraryViewModel dut;
        FakeLibrarySetRepo librarySetRepo;
        FakeWindowLauncher windowLauncher;
        FakeMetadataManager metadataManager;
        FakeDialogService dialogService;

        public LibraryViewModelTest()
        {
            dialogService = new FakeDialogService();
            librarySetRepo = new FakeLibrarySetRepo();
            windowLauncher = new FakeWindowLauncher();
            metadataManager = new FakeMetadataManager();
            dut = new LibraryViewModel(librarySetRepo, windowLauncher, metadataManager, dialogService);
        }

        [Fact]
        public void WhenMetadataDoesntExistInFolderCreatesIt()
        {
            metadataManager.ExistingMetadataFolders.Add("existing");
            windowLauncher.ToReturn.Name = "Test";
            windowLauncher.ToReturn.Path = "nonexisting";
            windowLauncher.ToReturn.Accepted = true;
            dut.SelectLibraryCommand.Execute(new LibraryItemViewModel { IsAddNew = true });
            Assert.Contains("nonexisting", metadataManager.ExistingMetadataFolders);
        }
    }
}
