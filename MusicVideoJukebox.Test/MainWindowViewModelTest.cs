using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    public class MainWindowViewModelTest
    {
        MainWindowViewModel dut;
        FakeMediaPlayer mediaPlayer;
        FakeSettingsWindowFactory settingsDialogFactory;
        FakeDialogService dialogService;
        FakeUIThreadFactory uIThreadFactory;
        FakeFileSystemService fileSystemService;
        FakeVideoLibraryBuilder videoLibraryBuilder;

        public MainWindowViewModelTest()
        {
            mediaPlayer = new FakeMediaPlayer();
            settingsDialogFactory = new FakeSettingsWindowFactory();
            dialogService = new FakeDialogService();
            fileSystemService = new FakeFileSystemService();
            videoLibraryBuilder = new FakeVideoLibraryBuilder();
            uIThreadFactory = new FakeUIThreadFactory();
            dut = new MainWindowViewModel(mediaPlayer, settingsDialogFactory, dialogService, uIThreadFactory, fileSystemService, videoLibraryBuilder);
        }
    }
}
