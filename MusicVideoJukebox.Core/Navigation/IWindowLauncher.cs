namespace MusicVideoJukebox.Core.Navigation
{
    public class AddLibraryDialogResult
    {
        bool Accepted { get; set; }
        int? LibraryId { get; set; }
    }

    public interface IWindowLauncher
    {
        AddLibraryDialogResult LaunchAddLibraryDialog();
    }
}
