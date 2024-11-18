using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeDialogService : IDialogService
    {
        public bool ShowedFolderSelect = false;
        public FolderPickerResult PickResultToReturn = new();

        public void ShowError(string message)
        {
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
