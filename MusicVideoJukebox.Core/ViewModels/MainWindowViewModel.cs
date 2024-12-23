using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private bool isLibrarySelected = false;
        private bool isPlaylistSelected = false;
        private IFadesWhenInactive interfaceFader = null!;
        private readonly INavigationService navigationService;
        private readonly LibraryStore libraryStore;

        public bool IsThereAValidLibraryActive => libraryStore.CurrentState.LibraryId != null;

        public bool IsLibrarySelected => navigationService.CurrentViewModel is LibraryViewModel;
        public bool IsPlaylistEditSelected => navigationService.CurrentViewModel is PlaylistEditViewModel;
        public bool IsPlaylistPlaySelected => navigationService.CurrentViewModel is PlaylistPlayViewModel;
        public bool IsMetadataSelected => navigationService.CurrentViewModel is MetadataEditViewModel;
        public bool IsAnalyzeSelected => navigationService.CurrentViewModel is AnalyzeViewModel;
        public ICommand NavigateLibraryCommand { get; }
        public ICommand NavigatePlaylistEditCommand { get; }
        public ICommand NavigateMetadataCommand { get; }
        public ICommand NavigatePlaylistPlayCommand { get; }
        public AsyncInitializeableViewModel? CurrentViewModel => navigationService.CurrentViewModel;

        public MainWindowViewModel(INavigationService navigationService, LibraryStore libraryStore)
        {
            NavigateLibraryCommand = new DelegateCommand(NavigateToLibrary);
            NavigatePlaylistEditCommand = new DelegateCommand(NavigateToPlaylistEdit);
            NavigateMetadataCommand = new DelegateCommand(NavigateToMetadata);
            NavigatePlaylistPlayCommand = new DelegateCommand(NavigateToPlaylistPlay);
            NavigateAnalyzeCommand = new DelegateCommand(NavigateAnalyze);
            this.navigationService = navigationService;
            this.libraryStore = libraryStore;
            navigationService.NavigationChanged += NavigationService_NavigationChanged;
        }

        public void RestoreState()
        {
            RefreshButtons();
        }

        void RefreshButtons()
        {
            OnPropertyChanged(nameof(IsLibrarySelected));
            OnPropertyChanged(nameof(IsPlaylistEditSelected));
            OnPropertyChanged(nameof(IsPlaylistPlaySelected));
            OnPropertyChanged(nameof(IsMetadataSelected));
            OnPropertyChanged(nameof(IsAnalyzeSelected));
            OnPropertyChanged(nameof(IsThereAValidLibraryActive));
        }

        private async void NavigateToPlaylistPlay()
        {
            if (IsPlaylistPlaySelected)
            {
                await navigationService.NavigateToNothing();
            }
            else
            {
                await navigationService.NavigateTo<PlaylistPlayViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void NavigationService_NavigationChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(IsThereAValidLibraryActive));
            RefreshButtons();
        }

        private async void NavigateToLibrary()
        {
            if (IsLibrarySelected)
            {
                await navigationService.NavigateToNothing();
            }
            else
            {
                await navigationService.NavigateTo<LibraryViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
            RefreshButtons();
        }

        private async void NavigateToPlaylistEdit()
        {
            if (IsPlaylistEditSelected)
            {
                await navigationService.NavigateToNothing();
            }
            else
            {
                await navigationService.NavigateTo<PlaylistEditViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
            RefreshButtons();
        }

        private async void NavigateToMetadata()
        {
            if (IsMetadataSelected)
            {
                await navigationService.NavigateToNothing();
            }
            else
            {
                await navigationService.NavigateTo<MetadataEditViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
            RefreshButtons();
        }

        public DelegateCommand NavigateAnalyzeCommand { get; }

        private async void NavigateAnalyze()
        {
            if (IsAnalyzeSelected)
            {
                await navigationService.NavigateToNothing();
            }
            else
            {
                await navigationService.NavigateTo<AnalyzeViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
            RefreshButtons();
        }
    }
}
