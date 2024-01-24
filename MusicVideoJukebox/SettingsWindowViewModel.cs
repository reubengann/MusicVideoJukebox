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
        private int selectedPlaylistIndex = 0;
        private LibraryMetadata? metadata;

        public string VideoFolderPath { get; set; }
        public bool MetadataNotFound { get; set; }
        public ObservableCollection<PlaylistViewModel> Playlists { get; set; }
        public string MetadataFoundString => MetadataNotFound ? "No" : "Yes";
        public int SelectedPlaylistIndex
        {
            get => selectedPlaylistIndex;
            set
            {
                selectedPlaylistIndex = value;
                if (metadata == null) { return; }

                var selectedPlaylist = Playlists[selectedPlaylistIndex];
                var videoInfoAndOrders = metadata.PlaylistMap[selectedPlaylist.PlaylistId];
                var videoIdToPlayOrderMap = videoInfoAndOrders.ToDictionary(x => x.Info.VideoId, x => x.PlayOrder);

                foreach (var ssvm in TrackListing)
                {
                    if (videoIdToPlayOrderMap.ContainsKey(ssvm.VideoId))
                    {
                        ssvm.Reset(true);
                        ssvm.Order = videoIdToPlayOrderMap[ssvm.VideoId];
                    }
                    else
                    {
                        ssvm.Reset(false);
                        ssvm.Order = 0;
                    }
                }
            }
        }
        public ObservableCollection<SettingsSongViewModel> TrackListing { get; set; }
        public bool HasSettingsToSave
        {
            get => hasSettingsToSave;
            set
            {
                hasSettingsToSave = value;
                OnPropertyChanged(nameof(HasSettingsToSave));
            }
        }
        public ICommand SaveChangesCommand => new DelegateCommand(SaveChanges);
        public ICommand AddNewPlaylistCommand => new DelegateCommand(AddNewPlaylist);

        private async void AddNewPlaylist()
        {
            using var updater = new MetadataUpdater(VideoFolderPath);
            var created = await updater.AddNewPlaylist("New playlist");
            Playlists.Add(new PlaylistViewModel(created.PlaylistName, created.PlaylistId));
        }

        private async void SaveChanges()
        {
            if (Playlists.Select(x => x.NameWasChanged).Any())
            {
                using var updater = new MetadataUpdater(VideoFolderPath);
                foreach (var changedPlaylist in Playlists.Where(x => x.NameWasChanged))
                {
                    await updater.UpdatePlaylistName(changedPlaylist.PlaylistId, changedPlaylist.ItemName);
                }
            }
            if (TrackListing.Select(x => x.IsModified).Any())
            {
                var selectedPlaylist = Playlists[selectedPlaylistIndex];
                // get added tracks
                foreach (var addedTrack in TrackListing.Where(x => x.WasAdded))
                {
                    Debug.WriteLine($"Item {addedTrack.VideoId} was added to playlist {selectedPlaylist.PlaylistId}");
                }
                // get removed tracks
                foreach (var removedTrack in TrackListing.Where(x => x.WasRemoved))
                {
                    Debug.WriteLine($"Item {removedTrack.VideoId} was removed from playlist {selectedPlaylist.PlaylistId}");
                }
            }
            HasSettingsToSave = false;
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
            metadata = await MetadataLoader.LoadAsync(VideoFolderPath);
            foreach (var playlist in metadata.Playlists)
            {
                var item = new PlaylistViewModel(playlist.PlaylistName, playlist.PlaylistId);
                Playlists.Add(item);
                item.ItemChanged += PlaylistNameChanged;
            }

            var settingsSongViewModels = new ObservableCollection<SettingsSongViewModel>(
                        metadata.VideoInfos.Select(track =>
                         new SettingsSongViewModel { VideoId = track.VideoId, Album = track.Album, Artist = track.Artist, Track = track.Title, Year = track.Year.ToString() })
                        );
            foreach (var songVm in settingsSongViewModels)
            {
                songVm.ItemWasChanged += SongVm_ItemWasChanged;
            }
            TrackListing = settingsSongViewModels;
            OnPropertyChanged(nameof(TrackListing));
            SelectedPlaylistIndex = 0;
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

    public class SettingsSongViewModel : BaseViewModel
    {
        public event Action? ItemWasChanged;

        private bool isActive;
        private int order;
        private bool originalState;

        public bool IsModified => isActive != originalState;
        public bool WasAdded => IsModified && (isActive && !originalState);
        public bool WasRemoved => IsModified && (!isActive && originalState);

        public void Reset(bool activeState)
        {
            originalState = activeState;
            isActive = activeState;
            OnPropertyChanged(nameof(IsActive));
        }
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                ItemWasChanged?.Invoke();
            }
        }
        public int VideoId { get; set; }
        public string Artist { get; set; } = null!;
        public string Track { get; set; } = null!;
        public string? Album { get; set; }
        public string? Year { get; set; }
        public int Order { get => order; set { order = value; OnPropertyChanged(nameof(Order)); } }
    }
}
