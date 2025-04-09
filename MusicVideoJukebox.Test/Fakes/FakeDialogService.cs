using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.UserInterface;
using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeDialogService : IDialogService
    {
        public bool ShowedFolderSelect = false;
        public bool ShowedError = false;
        public bool ShowedMatchDialog = false;
        public MetadataMatchDialogResult MatchDialogResultToReturn = new();
        public FolderPickerResult PickResultToReturn = new();
        public SingleFilePickerResult SingleFilePickerResultToReturn = new();
        public bool AcceptedDetailsResult = false;
        public Playlist AcceptedDetailsObject = new();

        public bool ShowedSingleFileSelect { get; internal set; }

        public MultipleFilePickerResult PickMultipleFiles(string filter)
        {
            throw new NotImplementedException();
        }

        public SingleFilePickerResult PickSingleFile(string filter)
        {
            ShowedSingleFileSelect = true;
            return SingleFilePickerResultToReturn; 
        }

        public void ShowEditPlaylistDetailsDialog(PlaylistDetailsEditDialogViewModel vm)
        {
            vm.Accepted = AcceptedDetailsResult;
            vm.PlaylistDescription = AcceptedDetailsObject.Description;
            vm.PlaylistIcon = AcceptedDetailsObject.ImagePath;
            vm.PlaylistName = AcceptedDetailsObject.PlaylistName;
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

        public MetadataMatchDialogResult ShowMatchDialog(MatchDialogViewModel vm)
        {
            ShowedMatchDialog = true;
            return MatchDialogResultToReturn;
        }

        public void ShowPopupPlaylist(PlaylistTrackSelectionViewModel vm)
        {
            throw new NotImplementedException();
        }

        public void ShutDownApp()
        {
        }
    }
}
