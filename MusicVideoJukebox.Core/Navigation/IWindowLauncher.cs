namespace MusicVideoJukebox.Core.Navigation
{
    public class AddLibraryDialogResult
    {
        public bool Accepted { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
    }

    public interface IWindowLauncher
    {
        AddLibraryDialogResult LaunchAddLibraryDialog();
    }
}
