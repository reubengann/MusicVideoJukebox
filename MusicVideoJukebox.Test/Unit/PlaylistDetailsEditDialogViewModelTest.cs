using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistDetailsEditDialogViewModelTest
    {
        PlaylistDetailsEditDialogViewModel dut;
        FakeDialogService dialogService;
        FakeImageScalerService imageScalerService;
        LibraryStore libraryStore;
        FakeLibrarySetRepo librarySetRepo;

        public PlaylistDetailsEditDialogViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            imageScalerService = new FakeImageScalerService();
            dialogService = new FakeDialogService();
            libraryStore = new LibraryStore(librarySetRepo);
            libraryStore.SetLibrary(1, @"c:\cool").Wait();
            dut = new PlaylistDetailsEditDialogViewModel(new Playlist { Description = "foobar", PlaylistId = 1, PlaylistName = "name" }, dialogService, imageScalerService, libraryStore);
        }

        [Fact]
        public void BasicProperties()
        {
            Assert.Equal("foobar", dut.PlaylistDescription);
            Assert.Equal("name", dut.PlaylistName);
        }

        [Fact]
        public void LaunchesFileSelection()
        {
            dut.SelectIconCommand.Execute(null);
            Assert.True(dialogService.ShowedSingleFileSelect);
        }

        [Fact]
        public void WhenGetsPngConvertsIt()
        {
            dialogService.SingleFilePickerResultToReturn.Accepted = true;
            dialogService.SingleFilePickerResultToReturn.SelectedFile = "file1.png";
            dut.SelectIconCommand.Execute(null);
            Assert.Contains("file1.png", imageScalerService.ScaledImages);
        }

        [Fact]
        public void ShowErrorWhenCantConvert()
        {
            imageScalerService.PathToReturn = null;
            dialogService.SingleFilePickerResultToReturn.Accepted = true;
            dialogService.SingleFilePickerResultToReturn.SelectedFile = "file1.png";
            dut.SelectIconCommand.Execute(null);
            Assert.True(dialogService.ShowedError);
        }

        [Fact]
        public void ReflectTheConvertedImageInTheUI()
        {
            imageScalerService.PathToReturn = @"images\somethingvalid";
            dialogService.SingleFilePickerResultToReturn.Accepted = true;
            dialogService.SingleFilePickerResultToReturn.SelectedFile = "file1.png";
            dut.SelectIconCommand.Execute(null);
            Assert.False(dialogService.ShowedError);
            Assert.Equal(@"c:\cool\images\somethingvalid", dut.PlaylistIcon);
        }

        [Fact]
        public void SetsSaveStatus()
        {
            var requestedClose = false;
            dut.RequestedClose += () => requestedClose = true;
            dut.SaveCommand.Execute(null);
            Assert.True(dut.Accepted);
            dut.CancelCommand.Execute(null);
            Assert.False(dut.Accepted);
            Assert.True(requestedClose);
        }
    }
}
