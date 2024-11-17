namespace MusicVideoJukebox.Core
{
    public class FolderPickerResult
    {
        public bool Accepted { get; set; }
        public string? SelectedFolder { get; set; }
    }

    public interface IDialogService
    {
        FolderPickerResult ShowFolderSelect(string title, string InitialDirectory);
        void ShowError(string message);
        void ShutDownApp();
    }
}
