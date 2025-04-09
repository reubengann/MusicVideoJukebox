using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{

    public class PlaylistTrackViewModelForPicker
    {
        public int PlayOrder { get; set; }
        required public string Artist { get; set; }
        required public string Title { get; set; }
    }


    public class PlaylistTrackSelectionViewModel :BaseViewModel
    {
        private readonly IMetadataManager metadataManager;
        public ICommand CancelCommand { get; set; }
        public ICommand OKCommand { get; set; }


        string filterText = string.Empty;
        public string FilterText { get => filterText; set { if (SetProperty(ref filterText, value)) FilterTracks(); } }

        private void FilterTracks()
        {
            FilteredTracks.Clear();
            foreach (var track in AllTracks)
            {
                if (string.IsNullOrEmpty(FilterText) 
                    || track.Title.ToLower().Contains(FilterText.ToLower()) || track.Artist.ToLower().Contains(FilterText.ToLower()))
                {
                    FilteredTracks.Add(track);
                }
            }
        }

        public PlaylistTrackViewModelForPicker? SelectedTrack { get; set; }

        private List<PlaylistTrackViewModelForPicker> AllTracks = [];
        public ObservableCollection<PlaylistTrackViewModelForPicker> FilteredTracks { get; set; } = [];

        public PlaylistTrackSelectionViewModel(IMetadataManager metadataManager)
        {
            this.metadataManager = metadataManager;
            CancelCommand = new DelegateCommand(Cancel);
            OKCommand = new DelegateCommand(Ok);
        }

        public bool Accepted { get; set; } = false;

        private void Ok()
        {
            Accepted = true;
            RequestedClose?.Invoke();
        }

        private void Cancel()
        {
            RequestedClose?.Invoke();
        }

        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public event Action? RequestedClose;

        internal async Task Load(int playlistId)
        {
            var tracks = await metadataManager.GetPlaylistTracksForViewmodel(playlistId);
            var currentTrack = await metadataManager.VideoRepo.GetActivePlaylist();
            foreach (var track in tracks)
            {
                AllTracks.Add(new PlaylistTrackViewModelForPicker
                {
                    PlayOrder = track.PlayOrder,
                    Title = track.Title,
                    Artist = track.Artist
                });
                if (track.PlayOrder == currentTrack.SongOrder)
                {
                    SelectedTrack = AllTracks.Last();
                }
            }
            FilterTracks();
        }
    }
}
