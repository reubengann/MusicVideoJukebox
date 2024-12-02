using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class NavigationServiceTest
    {
        NavigationService dut;
        FakeInterfaceFader interfaceFader;

        public NavigationServiceTest()
        {
            interfaceFader = new FakeInterfaceFader();
            var serviceProvider = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddSingleton<FakeAsyncInitializableViewModel>();
            }).Build().Services ?? throw new Exception();
            dut = new NavigationService(serviceProvider);
            dut.Initialize(interfaceFader);
        }

        [Fact]
        public void EnablesFadingWhenGoingToPlayerView()
        {
            interfaceFader.DisableFading();
            dut.NavigateToNothing();
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
