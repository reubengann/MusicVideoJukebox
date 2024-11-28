using MusicVideoJukebox.Core.ViewModels;
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
        FakeDialogService dialogService;
        FakeMetadataManagerFactory metadataManagerFactory;


        public LibraryViewModelTest()
        {
            dialogService = new FakeDialogService();
            librarySetRepo = new FakeLibrarySetRepo();
            windowLauncher = new FakeWindowLauncher();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            dut = new LibraryViewModel(librarySetRepo, windowLauncher, metadataManagerFactory, dialogService);
        }

        [Fact]
        public void WhenMetadataDoesntExistInFolderCreatesIt()
        {
            metadataManagerFactory.ToReturn.CreatedMetadataFolders.Add("existing");
            windowLauncher.ToReturn.Name = "Test";
            windowLauncher.ToReturn.Path = "nonexisting";
            windowLauncher.ToReturn.Accepted = true;
            dut.SelectLibraryCommand.Execute(new LibraryItemViewModel { IsAddNew = true });
            Assert.Contains("nonexisting", metadataManagerFactory.ToReturn.CreatedMetadataFolders);
            Assert.Single(librarySetRepo.LibraryItems);
        }
    }
}
