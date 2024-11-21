using Microsoft.Extensions.DependencyInjection;
using MusicVideoJukebox.Core;
using System;

namespace MusicVideoJukebox
{
    class WindowsSettingsWindowFactory : ISettingsWindowFactory
    {
        private readonly IServiceProvider serviceProvider;

        public WindowsSettingsWindowFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ISettingsWindow Create()
        {
            return serviceProvider.GetRequiredService<SettingsWindow>();
        }
    }
}
