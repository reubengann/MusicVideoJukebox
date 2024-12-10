namespace MusicVideoJukebox.Core.Navigation
{
    public interface INavigationService
    {
        AsyncInitializeableViewModel? CurrentViewModel { get; }
        Task NavigateTo<TViewModel>() where TViewModel : AsyncInitializeableViewModel;
        Task NavigateToNothing();
        event Action? NavigationChanged;
    }
}
