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

        public bool SelectedPressed { get; set; }
        public int WindowHeight { get => windowHeight; set => SetProperty(ref windowHeight, value); }
        public int WindowWidth { get => windowWidth; set => SetProperty(ref windowWidth, value); }
        public ObservableCollection<ScoredMetadata> Candidates { get; set; } = [];

        public ICommand SearchCommand { get; set; }

        public string SearchArtist { get => searchArtist; set => SetProperty(ref searchArtist, value); }
        public string SearchTitle { get => searchTitle; set => SetProperty(ref searchTitle, value); }
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public MatchDialogViewModel(VideoMetadata metadata, IMetadataManager metadataManager)
        {
            SearchCommand = new DelegateCommand(SearchAgain);
            this.metadata = metadata;
            this.metadataManager = metadataManager;
            searchArtist = metadata.Artist;
            searchTitle = metadata.Title;
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
