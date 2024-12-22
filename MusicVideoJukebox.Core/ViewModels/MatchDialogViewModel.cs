using MusicVideoJukebox.Core.Metadata;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MatchDialogViewModel : BaseViewModel
    {
        private readonly VideoMetadata metadata;
        private readonly IMetadataManager metadataManager;
        private string searchArtist;
        private string searchTitle;
        private bool isLoading;

        public bool SelectedPressed { get; set; }
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public ObservableCollection<ScoredMetadata> Candidates { get; set; } = [];

        public string SearchArtist { get => searchArtist; set => SetProperty(ref searchArtist, value); }
        public string SearchTitle { get => searchTitle; set => SetProperty(ref searchTitle, value); }
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public MatchDialogViewModel(VideoMetadata metadata, IMetadataManager metadataManager)
        {
            this.metadata = metadata;
            this.metadataManager = metadataManager;
            searchArtist = metadata.Artist;
            searchTitle = metadata.Title;
        }

        public async Task Initialize()
        {
            IsLoading = true;

            try
            {
                var candidates = await metadataManager.GetScoredCandidates(metadata.Artist, metadata.Title);
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
