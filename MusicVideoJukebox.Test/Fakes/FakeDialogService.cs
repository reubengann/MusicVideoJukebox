using MusicVideoJukebox.Core.UserInterface;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeDialogService : IDialogService
    {
        public bool ShowedFolderSelect = false;
        public bool ShowedError = false;
        public FolderPickerResult PickResultToReturn = new();

        public MultipleFilePickerResult PickMultipleFiles(string filter)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string message)
        {
            ShowedError = true;
        }

        public FolderPickerResult ShowFolderSelect(string title, string InitialDirectory)
        {
            ShowedFolderSelect = true;
            return PickResultToReturn;
        }

        public void ShutDownApp()
        {
        }
    }
}
