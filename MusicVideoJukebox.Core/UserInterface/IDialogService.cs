namespace MusicVideoJukebox.Core.UserInterface
{
    public class FolderPickerResult
    {
        public bool Accepted { get; set; }
        public string? SelectedFolder { get; set; }
    }

    public class MultipleFilePickerResult
    {
        public bool Accepted { get; set; }
        public List<string>? SelectedFiles { get; set; }
    }

    public interface IDialogService
    {
        FolderPickerResult ShowFolderSelect(string title, string InitialDirectory);
        MultipleFilePickerResult PickMultipleFiles(string filter);

        void ShowError(string message);
        void ShutDownApp();
    }
}
