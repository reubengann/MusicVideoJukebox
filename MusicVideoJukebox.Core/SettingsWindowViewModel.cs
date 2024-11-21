using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core
{
    public class SettingsWindowViewModel : BaseViewModel
    {
        private bool hasSettingsToSave;
        private int selectedPlaylistIndex = 0;
        private readonly VideoLibraryStore videoLibraryStore;
        private readonly IVideoLibraryBuilder videoLibraryBuilder;

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
                var selectedPlaylist = Playlists[selectedPlaylistIndex];
                var videoInfoAndOrders = videoLibraryStore.VideoLibrary.PlaylistIdToSongOrderMap[selectedPlaylist.PlaylistId];
                var videoIdToPlayOrderMap = videoInfoAndOrders.ToDictionary(x => x.Info.VideoId, x => x.PlayOrder);

                foreach (var ssvm in TrackListing)
                {
                    if (videoIdToPlayOrderMap.TryGetValue(ssvm.VideoId, out int order))
                    {
                        ssvm.Reset(true);
                        ssvm.Order = order;
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

        public ImportViewModel ImportViewModel { get; }

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
                var added = TrackListing.Where(x => x.WasAdded).Select(x => x.VideoId).ToList();
                var removed = TrackListing.Where(x => x.WasRemoved).Select(x => x.VideoId).ToList();
                using var updater = new MetadataUpdater(VideoFolderPath);
                await updater.UpdateTracksInPlaylist(selectedPlaylist.PlaylistId, added, removed);
            }
            videoLibraryStore.VideoLibrary = await videoLibraryBuilder.BuildAsync(videoLibraryStore.VideoLibrary.Folder);
            SelectedPlaylistIndex = selectedPlaylistIndex; // refreshes the listing
            HasSettingsToSave = false;
        }

        public SettingsWindowViewModel(VideoLibraryStore videoLibraryStore, IVideoLibraryBuilder videoLibraryBuilder, ImportViewModel importViewModel)
        {
            VideoFolderPath = videoLibraryStore.VideoLibrary.Folder;
            Playlists = [];
            TrackListing = [];
            this.videoLibraryStore = videoLibraryStore;
            this.videoLibraryBuilder = videoLibraryBuilder;
            ImportViewModel = importViewModel;
            foreach (var playlist in videoLibraryStore.VideoLibrary.Playlists)
            {
                var item = new PlaylistViewModel(playlist.PlaylistName, playlist.PlaylistId);
                Playlists.Add(item);
                item.ItemChanged += PlaylistNameChanged;
            }

            var settingsSongViewModels = new ObservableCollection<SettingsSongViewModel>(
                        videoLibraryStore.VideoLibrary.VideoInfos.Select(track =>
                         new SettingsSongViewModel { VideoId = track.VideoId, Album = track.Album, Artist = track.Artist, Track = track.Title, Year = track.Year.ToString() })
                        );
            foreach (var songVm in settingsSongViewModels)
            {
                songVm.ItemWasChanged += SongVm_ItemWasChanged;
            }
            TrackListing = settingsSongViewModels;
            //OnPropertyChanged(nameof(TrackListing));
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

    public class PlaylistViewModel(string itemName, int playlistId) : BaseViewModel
    {
        private string itemName = itemName;
        public int PlaylistId { get; private set; } = playlistId;

        public event Action? ItemChanged;
        public bool NameWasChanged { get; private set; } = false;

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
