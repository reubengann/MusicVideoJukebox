using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Navigation;

namespace MusicVideoJukebox.Test
{
    internal class FakeNavigationService : INavigationService
    {
        BaseViewModel? viewModel;
        public Dictionary<Type, BaseViewModel> ViewModelsToGenerate = [];

        public BaseViewModel? CurrentViewModel => viewModel;

        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            viewModel = ViewModelsToGenerate[typeof(TViewModel)];
        }

        public void NavigateToNothing()
        {
            viewModel = null;
        }
    }
}