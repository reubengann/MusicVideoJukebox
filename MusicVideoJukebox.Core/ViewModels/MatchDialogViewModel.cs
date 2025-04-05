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
        private bool isLoading;
        private int windowHeight;
        private int windowWidth;
        private SearchResult? selectedItem;
        public event Action? RequestClose;

        public int WindowHeight { get => windowHeight; set => SetProperty(ref windowHeight, value); }
        public int WindowWidth { get => windowWidth; set => SetProperty(ref windowWidth, value); }
        public ObservableCollection<SearchResult> Candidates { get; set; } = [];
        public SearchResult? SelectedItem { get => selectedItem; set { 
                SetProperty(ref selectedItem, value);
                if (selectedItem == null) return;
                EntryArtist = selectedItem.Artist;
                EntryTitle = selectedItem.Title;
                EntryAlbum = selectedItem.AlbumTitle ?? "";
                EntryYear = selectedItem.ReleaseYear?.ToString() ?? "";
            } 
        }

        private string queryString;
        public string QueryString { get => queryString; set => SetProperty(ref queryString, value); }
        
        public bool Accepted { get; set; } = false;

        public ICommand SearchCommand { get; }
        public ICommand CancelCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public VideoMetadata Metadata => metadata;

        public bool IsLoading { get => isLoading; set => SetProperty(ref isLoading, value); }

        public MatchDialogViewModel(VideoMetadata metadata, IMetadataManager metadataManager)
        {
            SearchCommand = new DelegateCommand(SearchAgain);
            SaveCommand = new DelegateCommand(Save, CanSave);
            CancelCommand = new DelegateCommand(Cancel);
            entryArtist = metadata.Artist;
            entryTitle = metadata.Title;
            entryAlbum = metadata.Album ?? "";
            entryYear = metadata.ReleaseYear?.ToString() ?? "";
            queryString = $"{metadata.Artist}%{metadata.Title}";
            this.metadata = metadata;
            this.metadataManager = metadataManager;
        }

        private void Cancel()
        {
            RequestClose?.Invoke();
        }

        private bool CanSave()
        {
            return !string.IsNullOrEmpty(EntryArtist)
                && !string.IsNullOrEmpty(EntryTitle)
                && (string.IsNullOrEmpty(EntryYear) || int.TryParse(EntryYear, out _));
        }


        private async void Save()
        {
            if (!CanSave())
            {
                return;
            }
            metadata.Artist = EntryArtist;
            metadata.Title = EntryTitle;
            metadata.Album = EntryAlbum;
            metadata.ReleaseYear = string.IsNullOrEmpty(EntryYear) ? null : int.Parse(EntryYear);
            metadata.Status = MetadataStatus.Done;
            await metadataManager.VideoRepo.UpdateMetadata(metadata);
            Accepted = true;
            RequestClose?.Invoke();
        }

        private async void SearchAgain()
        {
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
                var results = await metadataManager.SearchReferenceDb(QueryString);
                Candidates.Clear();
                foreach (var item in results)
                {
                    Candidates.Add(item);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string entryArtist;

        public string EntryArtist { get => entryArtist; set { SetProperty(ref entryArtist, value); SaveCommand.RaiseCanExecuteChanged(); } }

        private string entryTitle;

        public string EntryTitle { get => entryTitle; set { SetProperty(ref entryTitle, value); SaveCommand.RaiseCanExecuteChanged(); } }

        private string entryAlbum;

        public string EntryAlbum { get => entryAlbum; set => SetProperty(ref entryAlbum, value); }

        private string entryYear;

        public string EntryYear { get => entryYear; set { SetProperty(ref entryYear, value); SaveCommand.RaiseCanExecuteChanged(); } }
    }
}
