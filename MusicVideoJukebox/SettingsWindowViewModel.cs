using MusicVideoJukebox.Core;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicVideoJukebox
{
    class SettingsWindowViewModel : BaseViewModel
    {
        private bool hasSettingsToSave;

        public string VideoFolderPath { get; set; }
        public bool MetadataNotFound { get; set; }
        public ObservableCollection<PlaylistViewModel> Playlists { get; set; }
        public string MetadataFoundString => MetadataNotFound ? "No" : "Yes";
        public int SelectedPlaylistIndex { get; set; } = 0;
        public ObservableCollection<SettingsSongViewModel> TrackListing { get; set; }
        public bool HasSettingsToSave { get => hasSettingsToSave; set { hasSettingsToSave = value; OnPropertyChanged(nameof(HasSettingsToSave)); } }
        public ICommand SaveChangesCommand => new DelegateCommand(SaveChanges);
        public ICommand AddNewPlaylistCommand => new DelegateCommand(AddNewPlaylist);

        private async void AddNewPlaylist()
        {
            var created = await MetadataLoader.AddNewPlaylist(VideoFolderPath, "New playlist");
            Playlists.Add(new PlaylistViewModel(created.PlaylistName, created.PlaylistId));
        }

        private void SaveChanges()
        {
            if (Playlists.Select(x => x.NameWasChanged).Any())
            {
                foreach (var changedPlaylist in Playlists.Where(x => x.NameWasChanged))
                {
                    Debug.WriteLine($"Playlist {changedPlaylist.PlaylistId} name was changed to {changedPlaylist.ItemName}");
                }
            }
        }

        public SettingsWindowViewModel(string folder)
        {
            VideoFolderPath = folder;
            Playlists = new ObservableCollection<PlaylistViewModel>();
            TrackListing = new ObservableCollection<SettingsSongViewModel>();
        }

        public async Task LoadAllMetadataAsync()
        {
            if (!File.Exists(Path.Combine(VideoFolderPath, "meta.db")))
                throw new NotImplementedException();
            MetadataNotFound = false;
            OnPropertyChanged(nameof(MetadataFoundString));
            var metadata = await MetadataLoader.LoadAsync(VideoFolderPath);
            foreach (var playlist in metadata.Playlists)
            {
                PlaylistViewModel item = new PlaylistViewModel(playlist.PlaylistName, playlist.PlaylistId);
                Playlists.Add(item);
                item.ItemChanged += PlaylistNameChanged;
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

        private void PlaylistNameChanged()
        {
            HasSettingsToSave = true;
        }

        private void SongVm_ItemWasChanged()
        {
            HasSettingsToSave = true;
        }
    }

    public class PlaylistViewModel : BaseViewModel
    {
        private string itemName;
        public int PlaylistId { get; private set; }

        public event Action? ItemChanged;
        public bool NameWasChanged { get; private set; } = false;

        public PlaylistViewModel(string itemName, int playlistId)
        {
            this.itemName = itemName;
            this.PlaylistId = playlistId;
        }

        public string ItemName
        {
            get { return itemName; }
            set
            {
                if (itemName != value)
                {
                    itemName = value;
                    OnPropertyChanged(nameof(ItemName));
                    ItemChanged?.Invoke();
                    NameWasChanged = true;
                }
            }
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
