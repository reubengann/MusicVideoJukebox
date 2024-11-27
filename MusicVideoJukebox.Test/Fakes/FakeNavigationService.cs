using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Navigation;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeNavigationService : INavigationService
    {
        AsyncInitializeableViewModel? viewModel;
        public Dictionary<Type, AsyncInitializeableViewModel> ViewModelsToGenerate = [];

        public AsyncInitializeableViewModel? CurrentViewModel => viewModel;

        public void NavigateToNothing()
        {
            viewModel = null;
        }

        async Task INavigationService.NavigateTo<TViewModel>()
        {
            viewModel = ViewModelsToGenerate[typeof(TViewModel)];
        }
    }
}