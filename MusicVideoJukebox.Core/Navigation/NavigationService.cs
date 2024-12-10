using Microsoft.Extensions.DependencyInjection;
using MusicVideoJukebox.Core.UserInterface;
using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Core.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private IFadesWhenInactive fadesWhenInactive;
        private readonly VideoPlayingViewModel videoPlayingViewModel;
        private AsyncInitializeableViewModel? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider, IFadesWhenInactive fadesWhenInactive, VideoPlayingViewModel videoPlayingViewModel)
        {
            _serviceProvider = serviceProvider;
            this.fadesWhenInactive = fadesWhenInactive;
            this.videoPlayingViewModel = videoPlayingViewModel;
        }

        public AsyncInitializeableViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
            }
        }

        public event Action? NavigationChanged;

        public async Task NavigateTo<TViewModel>() where TViewModel : AsyncInitializeableViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            if (viewModel == null)
            {
                throw new InvalidOperationException($"Unable to resolve ViewModel of type {typeof(TViewModel).Name}");
            }
            await viewModel.Initialize();
            CurrentViewModel = viewModel;
            NavigationChanged?.Invoke();
            fadesWhenInactive.DisableFading();
        }

        public async Task NavigateToNothing()
        {
            CurrentViewModel = null;
            NavigationChanged?.Invoke();
            fadesWhenInactive.EnableFading();
            await videoPlayingViewModel.Recheck();
        }
    }
}
