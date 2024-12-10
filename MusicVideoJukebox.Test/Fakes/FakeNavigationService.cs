using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.UserInterface;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeNavigationService : INavigationService
    {
        public AsyncInitializeableViewModel? viewModel;
        public Dictionary<Type, AsyncInitializeableViewModel> ViewModelsToGenerate = [];

        public AsyncInitializeableViewModel? CurrentViewModel => viewModel;

        public event Action? NavigationChanged;

        public void Initialize(IFadesWhenInactive fadesWhenInactive)
        {
            throw new NotImplementedException();
        }

        async Task INavigationService.NavigateTo<TViewModel>()
        {
            await Task.CompletedTask;
            viewModel = ViewModelsToGenerate[typeof(TViewModel)];
        }

        async Task INavigationService.NavigateToNothing()
        {
            await Task.CompletedTask;
            viewModel = null;
        }
    }
}