using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MatchDialogViewModel : BaseViewModel
    {
        private readonly VideoMetadata metadata;
        private readonly IMetadataManager metadataManager;
        private string searchArtist;
        private string searchTitle;
        private bool isLoading;
        private int windowHeight;
        private int windowWidth;
        private ScoredMetadata? selectedItem;
        public event Action? RequestClose;

        public int WindowHeight { get => windowHeight; set => SetProperty(ref windowHeight, value); }
        public int WindowWidth { get => windowWidth; set => SetProperty(ref windowWidth, value); }
        public ObservableCollection<ScoredMetadata> Candidates { get; set; } = [];
        public ScoredMetadata? SelectedItem { get => selectedItem; set { SetProperty(ref selectedItem, value); SelectCommand.RaiseCanExecuteChanged(); } }

        public bool Accepted { get; set; } = false;

        public ICommand SearchCommand { get; }
        public ICommand CancelCommand { get; }
        public DelegateCommand SelectCommand { get; }

        public string SearchArtist { get => searchArtist; set { SetProperty(ref searchArtist, value); } }
        public string SearchTitle { get => searchTitle; set => SetProperty(ref searchTitle, value); }
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public MatchDialogViewModel(VideoMetadata metadata, IMetadataManager metadataManager)
        {
            SearchCommand = new DelegateCommand(SearchAgain);
            SelectCommand = new DelegateCommand(Select, CanSelect);
            CancelCommand = new DelegateCommand(Cancel);
            this.metadata = metadata;
            this.metadataManager = metadataManager;
            searchArtist = metadata.Artist;
            searchTitle = metadata.Title;
        }

        private void Cancel()
        {
            RequestClose?.Invoke();
        }

        private bool CanSelect()
        {
            return SelectedItem != null;
        }

        private void Select()
        {
            Accepted = true;
            RequestClose?.Invoke();
        }

        private async void SearchAgain()
        {
            Candidates.Clear();
            await DoTheSearch();
        }

        public async Task Initialize()
        {
            await DoTheSearch();
        }

        private async Task DoTheSearch()
        {
            IsLoading = true;

            try
            {
                var maybeExactMatch = await metadataManager.TryGetAlbumYear(SearchArtist, SearchTitle);
                if (maybeExactMatch.Success)
                {
                    Candidates.Add(new ScoredMetadata { AlbumTitle = maybeExactMatch.AlbumTitle, ArtistName = SearchArtist, FirstReleaseDateYear = maybeExactMatch.ReleaseYear, Similarity = 100, TrackName = SearchTitle });
                }
                var candidates = await metadataManager.GetScoredCandidates(SearchArtist, SearchTitle);
                foreach (var candidate in candidates.OrderByDescending(x => x.Similarity))
                {
                    Candidates.Add(candidate);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
