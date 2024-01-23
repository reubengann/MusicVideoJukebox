using MusicVideoJukebox.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            ObservableCollection<SettingsSongViewModel> settingsSongViewModels = new ObservableCollection<SettingsSongViewModel>(
                        metadata.VideoInfos.Select(track =>
                         new SettingsSongViewModel { VideoId = track.VideoId, Album = track.Album, Artist = track.Artist, Track = track.Title, Year = track.Year.ToString() })
                        );
            foreach (var songVm in settingsSongViewModels)
            {
                songVm.ItemWasChanged += SongVm_ItemWasChanged;
            }
            TrackListing = settingsSongViewModels;
            OnPropertyChanged(nameof(TrackListing));
        }

        private void SongVm_ItemWasChanged()
        {
            Debug.WriteLine("Item was changed");
        }
    }

    public class SettingsSongViewModel
    {
        public event Action? ItemWasChanged;

        private bool isActive;
        public bool IsModified { get; private set; } = false;

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                IsModified = true;
                ItemWasChanged?.Invoke();
            }
        }
        public int VideoId { get; set; }
        public string Artist { get; set; } = null!;
        public string Track { get; set; } = null!;
        public string? Album { get; set; }
        public string? Year { get; set; }
        public int Order { get; set; }
    }
}
