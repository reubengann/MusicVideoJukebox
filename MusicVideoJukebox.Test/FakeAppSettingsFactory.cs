using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeIAppSettings : IAppSettings
    {
        public string? Path = "";

        public string? VideoLibraryPath { get => Path; set => Path = value; }

        public Task Save()
        {
            throw new NotImplementedException();
        }

        public void UpdateVideoLibraryPath(string folderName)
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeAppSettingsFactory : IAppSettingsFactory
    {
        FakeIAppSettings Settings = new();

        public async Task<IAppSettings> Create()
        {
            await Task.CompletedTask;
            return Settings;
        }
    }
}