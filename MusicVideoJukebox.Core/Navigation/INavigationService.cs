namespace MusicVideoJukebox.Core.Navigation
{
    public interface INavigationService
    {
        AsyncInitializeableViewModel? CurrentViewModel { get; }
        Task NavigateTo<TViewModel>() where TViewModel : AsyncInitializeableViewModel;
        void NavigateToNothing();
        event Action? NavigationChanged;
    }
}
