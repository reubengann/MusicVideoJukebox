using System.Text.Json;

namespace MusicVideoJukebox.Core
{
    public interface IAppSettingsFactory
    {
        Task<IAppSettings> Create();
    }

    public class FileAppSettingsFactory : IAppSettingsFactory
    {
        public async Task<IAppSettings> Create()
        {
            return await FileAppSettings.Create();
        }
    }

    public class FileAppSettings : IAppSettings
    {
        private class AppSettingData
        {
            public string? VideoLibraryPath { get; set; }
        }

        private const string SettingsFilePath = "appsettings.json";
        private readonly AppSettingData data;
        static JsonSerializerOptions options = new() { WriteIndented = true };

        private FileAppSettings(AppSettingData data)
        {
            this.data = data;
        }

        public static async Task<FileAppSettings> Create()
        {
            AppSettingData settingData;

            if (File.Exists(SettingsFilePath))
            {
                var json = await File.ReadAllTextAsync(SettingsFilePath);
                settingData = JsonSerializer.Deserialize<AppSettingData>(json) ?? new AppSettingData();
            }
            else
            {
                settingData = new AppSettingData();
            }
            return new FileAppSettings(settingData);
        }

        public async Task Save()
        {
            var json = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }

        public void UpdateVideoLibraryPath(string folderName)
        {
            data.VideoLibraryPath = folderName;
        }

        public string? VideoLibraryPath { get => data.VideoLibraryPath; set => data.VideoLibraryPath = value; }
    }


    public class AppSettingsStore
    {
        public IAppSettings Settings { get; }

        public AppSettingsStore(IAppSettings settings)
        {
            Settings = settings;
        }
    }
}
