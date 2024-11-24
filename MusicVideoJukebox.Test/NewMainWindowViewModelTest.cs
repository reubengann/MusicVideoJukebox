using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    public class NewMainWindowViewModelTest
    {
        NewMainWindowViewModel dut;

        public NewMainWindowViewModelTest()
        {
            dut = new NewMainWindowViewModel();
        }

        [Fact]
        public void NothingIsSelectedInitially()
        {
            Assert.False(dut.IsLibrarySelected);
            Assert.False(dut.IsPlaylistSelected);
            Assert.False(dut.IsMetadataSelected);
        }

        [Fact]
        public void IfSelectedLibraryEnableIt()
        {
            dut.NavigateLibraryCommand.Execute(null);
            Assert.True(dut.IsLibrarySelected);
        }

        [Fact]
        public void IfSelectedLibraryAgainDisableIt()
        {
            dut.NavigateLibraryCommand.Execute(null);
            Assert.True(dut.IsLibrarySelected);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.False(dut.IsLibrarySelected);
        }

        [Fact]
        public void IfSelectedPlaylistAgainDisableIt()
        {
            dut.NavigatePlaylistCommand.Execute(null);
            Assert.True(dut.IsPlaylistSelected);
            dut.NavigatePlaylistCommand.Execute(null);
            Assert.False(dut.IsPlaylistSelected);
        }

        [Fact]
        public void IfSelectedMetadataAgainDisableIt()
        {
            dut.NavigateMetadataCommand.Execute(null);
            Assert.True(dut.IsMetadataSelected);
            dut.NavigateMetadataCommand.Execute(null);
            Assert.False(dut.IsMetadataSelected);
        }
    }
}
