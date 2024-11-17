using MusicVideoJukebox.Core;

namespace MusicVideoJukebox
{
    public interface ISettingsWindow
    {
        void ShowDialog();
        bool Result { get; set; }
    }

    public interface ISettingsWindowFactory
    {
        ISettingsWindow Create(VideoLibraryStore videoLibraryStore);
    }
}
