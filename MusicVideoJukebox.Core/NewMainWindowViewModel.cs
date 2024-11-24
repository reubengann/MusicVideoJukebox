using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core
{
    public class NewMainWindowViewModel : BaseViewModel
    {
        private bool isLibrarySelected = false;
        private bool isPlaylistSelected = false;

        public bool IsLibrarySelected { get => isLibrarySelected; set { isLibrarySelected = value; OnPropertyChanged(nameof(IsLibrarySelected)); } }
        public bool IsPlaylistSelected { get => isPlaylistSelected; set { isPlaylistSelected = value; OnPropertyChanged(nameof(IsPlaylistSelected)); } }
        public bool IsMetadataSelected { get; set; } = false;
        public ICommand NavigateLibraryCommand { get; }
        public ICommand NavigatePlaylistCommand { get; }
        public ICommand NavigateMetadataCommand { get; }

        public NewMainWindowViewModel()
        {
            NavigateLibraryCommand = new DelegateCommand(NavigateToLibrary);
            NavigatePlaylistCommand = new DelegateCommand(NavigateToPlaylist);
            NavigateMetadataCommand = new DelegateCommand(NavigateToMetadata);
        }

        private void NavigateToLibrary()
        {
            if (IsLibrarySelected) 
            { 
                IsLibrarySelected = false;
                return;
            }
            IsLibrarySelected = true;
            IsPlaylistSelected = false;
            IsMetadataSelected = false;
        }

        private void NavigateToPlaylist()
        {
            if (IsPlaylistSelected)
            {
                IsPlaylistSelected = false;
                return;
            }
            IsLibrarySelected = false;
            IsPlaylistSelected = true;
            IsMetadataSelected = false;
        }

        private void NavigateToMetadata()
        {
            if (IsMetadataSelected) { IsMetadataSelected = false; return; }
            IsLibrarySelected = false;
            IsPlaylistSelected = false;
            IsMetadataSelected = true;
        }
    }
}