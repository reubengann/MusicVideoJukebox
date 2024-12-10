using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class NavigationServiceTest
    {
        NavigationService dut;
        FakeInterfaceFader interfaceFader;
        VideoPlayingViewModel vm;
        FakeUIThreadFactory iuiThreadFactory;

        public NavigationServiceTest()
        {
            interfaceFader = new FakeInterfaceFader();
            iuiThreadFactory = new FakeUIThreadFactory();
            iuiThreadFactory.ToReturn.Add(new FakeUiThreadTimer());
            vm = new VideoPlayingViewModel(new FakeMediaPlayer2(), iuiThreadFactory, new FakeMetadataManagerFactory(), new LibraryStore());
            var serviceProvider = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddSingleton<FakeAsyncInitializableViewModel>();
            }).Build().Services ?? throw new Exception();
            dut = new NavigationService(serviceProvider, interfaceFader, vm);
        }

        [Fact]
        public async Task EnablesFadingWhenGoingToPlayerView()
        {
            interfaceFader.DisableFading();
            await dut.NavigateToNothing();
            Assert.True(interfaceFader.FadingEnabled);
        }

        [Fact]
        public async Task DisablesFadingWhenGoingToOtherView()
        {
            interfaceFader.EnableFading();
            await dut.NavigateTo<FakeAsyncInitializableViewModel>();
            Assert.False(interfaceFader.FadingEnabled);
        }
    }

    class FakeAsyncInitializableViewModel : AsyncInitializeableViewModel
    {
        public override Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
