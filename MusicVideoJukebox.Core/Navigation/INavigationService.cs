namespace MusicVideoJukebox.Core.Navigation
{
    public interface INavigationService
    {
        BaseViewModel? CurrentViewModel { get; }
        void NavigateTo<TViewModel>() where TViewModel : BaseViewModel;
        void NavigateToNothing();
    }
}
