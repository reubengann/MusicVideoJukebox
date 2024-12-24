using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistDetailsEditDialogViewModelTest
    {
        PlaylistDetailsEditDialogViewModel dut;

        public PlaylistDetailsEditDialogViewModelTest()
        {
            dut = new PlaylistDetailsEditDialogViewModel(new Playlist { });
        }
    }
}
