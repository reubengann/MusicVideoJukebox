using System.Collections.ObjectModel;

namespace MusicVideoJukebox
{
    class SettingsWindowDesignTimeViewModel
    {
        public ObservableCollection<SettingsSongViewModel> TrackListing { get; set; }
        public string VideoFolderPath { get; set; } = @"C:\music_videos\";
        public bool MetadataNotFound { get; set; } = false;
        public string MetadataFoundString => MetadataNotFound ? "No" : "Yes";

        public ObservableCollection<string> Playlists { get; set; }
        public int SelectedPlaylistIndex { get; set; } = 0;

        public SettingsWindowDesignTimeViewModel()
        {
            TrackListing = new ObservableCollection<SettingsSongViewModel>
            {
                new SettingsSongViewModel { Artist = "10,000 Maniacs", Track = "Because the Night [Unplugged]", Album = "Unplugged", Year = "1991", IsActive = true },
                new SettingsSongViewModel { Artist = "A Flock of Seagulls", Track = "I Ran", Album = "whatever", Year = "1982" },
            };
            Playlists = new ObservableCollection<string> { "All alphabetical" };
        }
    }

    public class SettingsSongViewModel
    {
        public bool IsActive { get; set; }
        public string Artist { get; set; } = null!;
        public string Track { get; set; } = null!;
        public string? Album { get; set; }
        public string? Year { get; set; }
    }
}
