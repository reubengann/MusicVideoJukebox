using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeDialogService : IDialogService
    {
        public void ShowError(string message)
        {
        }

        public FolderPickerResult ShowFolderSelect(string title, string InitialDirectory)
        {
            return new FolderPickerResult();
        }

        public void ShutDownApp()
        {
        }
    }
}
