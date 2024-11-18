using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeDialogService : IDialogService
    {
        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }

        public FolderPickerResult ShowFolderSelect(string title, string InitialDirectory)
        {
            throw new NotImplementedException();
        }

        public void ShutDownApp()
        {
            throw new NotImplementedException();
        }
    }
}
