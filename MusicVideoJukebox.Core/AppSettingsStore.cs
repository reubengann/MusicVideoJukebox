using System.Text.Json;

namespace MusicVideoJukebox.Core
{
    public class AppSettingsStore
    {
        private class AppSettings
        {
            public string? VideoLibraryPath { get; set; }
        }

        private const string SettingsFilePath = "appsettings.json";
        private AppSettings _settings;

        public string? VideoLibraryPath => _settings?.VideoLibraryPath;

        private AppSettingsStore(AppSettings settings)
        {
            _settings = settings;
        }

        public static async Task<AppSettingsStore> Create()
        {
            AppSettings settings;

            if (File.Exists(SettingsFilePath))
            {
                var json = await File.ReadAllTextAsync(SettingsFilePath);
                settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            else
            {
                settings = new AppSettings();
            }
            return new AppSettingsStore(settings);
        }

        public void UpdateVideoLibraryPath(string folderName)
        {
            _settings.VideoLibraryPath = folderName;
        }

        public async Task Save()
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }
    }
}
