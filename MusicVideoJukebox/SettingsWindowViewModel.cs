using System;
using System.IO;
using System.Threading.Tasks;

namespace MusicVideoJukebox
{
    class SettingsWindowViewModel : BaseViewModel
    {
        /*
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
                new SettingsSongViewModel { Artist = "10,000 Maniacs", Track = "Because the Night [Unplugged]", Album = "Unplugged", Year = "1991", IsActive = true, Order = 1 },
                new SettingsSongViewModel { Artist = "A Flock of Seagulls", Track = "I Ran", Album = "whatever", Year = "1982", Order = 2 },
            };
            Playlists = new ObservableCollection<string> { "All alphabetical" };
        }
         */
        private string folder;

        public SettingsWindowViewModel(string folder)
        {
            this.folder = folder;
        }

        public async Task LoadAllMetadataAsync()
        {
            if (!File.Exists(Path.Combine(folder, "meta.db")))
                throw new NotImplementedException();
        }
    }
}
