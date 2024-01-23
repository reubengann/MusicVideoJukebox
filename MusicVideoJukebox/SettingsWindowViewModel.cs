using MusicVideoJukebox.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicVideoJukebox
{
    class SettingsWindowViewModel : BaseViewModel
    {
        public string VideoFolderPath { get; set; }
        public bool MetadataNotFound { get; set; }
        public ObservableCollection<string> Playlists { get; set; }
        public string MetadataFoundString => MetadataNotFound ? "No" : "Yes";
        public int SelectedPlaylistIndex { get; set; } = 0;
        public ObservableCollection<SettingsSongViewModel> TrackListing { get; set; }

        public SettingsWindowViewModel(string folder)
        {
            VideoFolderPath = folder;
            Playlists = new ObservableCollection<string>();
            TrackListing = new ObservableCollection<SettingsSongViewModel>();
        }

        public async Task LoadAllMetadataAsync()
        {
            if (!File.Exists(Path.Combine(VideoFolderPath, "meta.db")))
                throw new NotImplementedException();
            MetadataNotFound = false;
            OnPropertyChanged(nameof(MetadataFoundString));
            var metadata = await MetadataLoader.LoadAsync(VideoFolderPath);
            foreach (var playlistName in metadata.PlaylistNames)
            {
                Playlists.Add(playlistName);
            }
            TrackListing = new ObservableCollection<SettingsSongViewModel>(
            metadata.VideoInfos.Select(track => new SettingsSongViewModel { Album = track.Album, Artist = track.Artist, Track = track.Title, Year = track.Year.ToString() })
            );
            OnPropertyChanged(nameof(TrackListing));
        }
    }
}
