using MusicVideoJukebox.Core.Navigation;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core
{
    public class NewMainWindowViewModel : BaseViewModel
    {
        private bool isLibrarySelected = false;
        private bool isPlaylistSelected = false;
        private readonly INavigationService navigationService;

        public bool IsLibrarySelected => navigationService.CurrentViewModel is LibraryViewModel;
        public bool IsPlaylistSelected => navigationService.CurrentViewModel is NewPlaylistViewModel;
        public bool IsMetadataSelected => navigationService.CurrentViewModel is MetadataEditViewModel;
        public ICommand NavigateLibraryCommand { get; }
        public ICommand NavigatePlaylistCommand { get; }
        public ICommand NavigateMetadataCommand { get; }
        public BaseViewModel? CurrentViewModel => navigationService.CurrentViewModel;

        public NewMainWindowViewModel(INavigationService navigationService)
        {
            NavigateLibraryCommand = new DelegateCommand(NavigateToLibrary);
            NavigatePlaylistCommand = new DelegateCommand(NavigateToPlaylist);
            NavigateMetadataCommand = new DelegateCommand(NavigateToMetadata);
            this.navigationService = navigationService;
        }

        private void NavigateToLibrary()
        {
            if (IsLibrarySelected) 
            {
                navigationService.NavigateToNothing();
            }
            else
            {
                navigationService.NavigateTo<LibraryViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void NavigateToPlaylist()
        {
            if (IsPlaylistSelected)
            {
                navigationService.NavigateToNothing();
            }
            else
            {
                navigationService.NavigateTo<NewPlaylistViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void NavigateToMetadata()
        {
            if (IsMetadataSelected) 
            {
                navigationService.NavigateToNothing();
            }
            else
            {
                navigationService.NavigateTo<MetadataEditViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}