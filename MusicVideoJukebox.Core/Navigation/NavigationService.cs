using Microsoft.Extensions.DependencyInjection;

namespace MusicVideoJukebox.Core.Navigation
{
    public class NavigationService : INavigationService//, INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private AsyncInitializeableViewModel? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public AsyncInitializeableViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
            }
        }

        public async Task NavigateTo<TViewModel>() where TViewModel : AsyncInitializeableViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            if (viewModel == null)
            {
                throw new InvalidOperationException($"Unable to resolve ViewModel of type {typeof(TViewModel).Name}");
            }
            await viewModel.Initialize();
            CurrentViewModel = viewModel;
        }

        public void NavigateToNothing()
        {
            CurrentViewModel = null;
        }
    }
}
