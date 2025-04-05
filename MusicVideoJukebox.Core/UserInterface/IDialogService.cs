using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;

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

    public class SingleFilePickerResult
    {
        public bool Accepted { get; set; }
        public string? SelectedFile { get; set; }
    }

    public class MetadataMatchDialogResult
    {
        public bool Accepted { get; set; }
        public VideoMetadata? ScoredMetadata { get; set; }
    }

    public interface IDialogService
    {
        FolderPickerResult ShowFolderSelect(string title, string InitialDirectory);
        MultipleFilePickerResult PickMultipleFiles(string filter);
        SingleFilePickerResult PickSingleFile(string filter);
        MetadataMatchDialogResult ShowMatchDialog(MatchDialogViewModel vm);
        void ShowEditPlaylistDetailsDialog(PlaylistDetailsEditDialogViewModel vm);

        void ShowError(string message);
        void ShutDownApp();
    }
}
