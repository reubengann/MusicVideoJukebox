using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class NewMainWindowViewModel : BaseViewModel
    {
        private bool isLibrarySelected = false;
        private bool isPlaylistSelected = false;
        private IFadesWhenInactive interfaceFader = null!;
        private readonly INavigationService navigationService;

        public bool IsLibrarySelected => navigationService.CurrentViewModel is LibraryViewModel;
        public bool IsPlaylistSelected => navigationService.CurrentViewModel is PlaylistEditViewModel;
        public bool IsMetadataSelected => navigationService.CurrentViewModel is MetadataEditViewModel;
        public ICommand NavigateLibraryCommand { get; }
        public ICommand NavigatePlaylistCommand { get; }
        public ICommand NavigateMetadataCommand { get; }
        public AsyncInitializeableViewModel? CurrentViewModel => navigationService.CurrentViewModel;

        public NewMainWindowViewModel(INavigationService navigationService)
        {
            NavigateLibraryCommand = new DelegateCommand(NavigateToLibrary);
            NavigatePlaylistCommand = new DelegateCommand(NavigateToPlaylist);
            NavigateMetadataCommand = new DelegateCommand(NavigateToMetadata);
            this.navigationService = navigationService;
            navigationService.NavigationChanged += NavigationService_NavigationChanged;
        }

        private void NavigationService_NavigationChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
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
        }

        private async void NavigateToPlaylist()
        {
            if (IsPlaylistSelected)
            {
                await navigationService.NavigateToNothing();
            }
            else
            {
                await navigationService.NavigateTo<PlaylistEditViewModel>();
            }
            OnPropertyChanged(nameof(CurrentViewModel));
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
        }

        public void Initialize(IFadesWhenInactive interfaceFader)
        {
            this.interfaceFader = interfaceFader;
        }
    }
}