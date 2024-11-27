using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeIAppSettings : IAppSettings
    {
        public string? Path = null;
        public bool StoredAPath = false;

        public string? VideoLibraryPath { get => Path; set => Path = value; }

        public Task Save()
        {
            return Task.CompletedTask;
        }

        public void UpdateVideoLibraryPath(string folderName)
        {
            Path = folderName;
            StoredAPath = true;
        }
    }

    internal class FakeAppSettingsFactory : IAppSettingsFactory
    {
        public FakeIAppSettings Settings = new();

        public async Task<IAppSettings> Create()
        {
            await Task.CompletedTask;
            return Settings;
        }
    }
}