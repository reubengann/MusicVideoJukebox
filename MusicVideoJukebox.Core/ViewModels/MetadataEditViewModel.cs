using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class VideoMetadataViewModel : BaseViewModel
    {
        private readonly VideoMetadata metadata;
        public VideoMetadata VideoMetadata => metadata;
        public VideoMetadataViewModel(VideoMetadata metadata)
        {
            this.metadata = metadata;
        }

        public string Title { get => metadata.Title; set => SetUnderlyingProperty(metadata.Title, value, v => metadata.Title = v); }
        public string Artist
        {
            get => metadata.Artist;
            set => SetUnderlyingProperty(metadata.Artist, value, v => metadata.Artist = v);
        }
        public string? Album
        {
            get => metadata.Album;
            set => SetUnderlyingProperty(metadata.Album, value, v => metadata.Album = v);
        }
        public int? ReleaseYear
        {
            get => metadata.ReleaseYear;
            set => SetUnderlyingProperty(metadata.ReleaseYear, value, v => metadata.ReleaseYear = v);
        }
        public MetadataStatus Status
        {
            get => metadata.Status;
            set => SetUnderlyingProperty(metadata.Status, value, v => metadata.Status = v);
        }

        internal void RefreshUI()
        {
            OnPropertyChanged(nameof(Artist));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Album));
            OnPropertyChanged(nameof(ReleaseYear));
            OnPropertyChanged(nameof(Status));
        }
    }

    public class MetadataEditViewModel : AsyncInitializeableViewModel
    {
        private ObservableCollection<VideoMetadataViewModel> metadataEntries = new();
        private ObservableCollection<VideoMetadataViewModel> filteredMetadataEntries = new();

        private bool hideCompleteEntries;
        public bool HideCompleteEntries
        {
            get => hideCompleteEntries;
            set
            {
                if (SetProperty(ref hideCompleteEntries, value))
                {
                    FilterMetadataEntries();
                }
            }
        }

        private void FilterMetadataEntries()
        {
            FilteredMetadataEntries.Clear();
            if (HideCompleteEntries)
            {
                foreach (var entry in metadataEntries.Where(entry => entry.Status == MetadataStatus.Done))
                {
                    FilteredMetadataEntries.Add(entry);
                }
            }
            else
            {
                foreach (var entry in metadataEntries)
                {
                    FilteredMetadataEntries.Add(entry);
                }
            }
        }


        public ObservableCollection<VideoMetadataViewModel> FilteredMetadataEntries
        {
            get => filteredMetadataEntries;
            set => SetProperty(ref filteredMetadataEntries, value);
        }

        public bool IsBusy { get => isBusy; set { isBusy = value; SetProperty(ref isBusy, value); } }

        public ICommand EditMetadataCommand { get; }
        public ICommand RefreshDatabaseCommand { get; }
        public ICommand EditAllUndoneCommand { get; }

        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;
        private readonly IDialogService dialogService;
        private IMetadataManager metadataManager = null!;
        private bool isBusy;
        private VideoMetadataViewModel? selectedItem;

        public VideoMetadataViewModel? SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public MetadataEditViewModel(IMetadataManagerFactory metadataManagerFactory, LibraryStore libraryStore, IDialogService dialogService)
        {
            EditMetadataCommand = new DelegateCommand(LaunchMatchDialog);
            RefreshDatabaseCommand = new DelegateCommand(async () => await ReconcileCurrentFolderContents());
            EditAllUndoneCommand = new DelegateCommand(DoAllUndone);
            this.metadataManagerFactory = metadataManagerFactory;
            this.libraryStore = libraryStore;
            this.dialogService = dialogService;
        }

        private void DoAllUndone()
        {
            foreach (var entry in metadataEntries.Where(entry => entry.Status != MetadataStatus.Done))
            {
                var result = dialogService.ShowMatchDialog(new MatchDialogViewModel(entry.VideoMetadata, metadataManager));
                if (result.Accepted)
                {
                    ArgumentNullException.ThrowIfNull(result.ScoredMetadata);
                    var scoredMetadata = result.ScoredMetadata;
                    entry.RefreshUI();
                }
                else
                {
                    return;
                }
            }
        }

        private void LaunchMatchDialog()
        {
            if (SelectedItem == null) return;
            var result = dialogService.ShowMatchDialog(new MatchDialogViewModel(SelectedItem.VideoMetadata, metadataManager));
            if (result.Accepted)
            {
                ArgumentNullException.ThrowIfNull(result.ScoredMetadata);
                var scoredMetadata = result.ScoredMetadata;
                SelectedItem.RefreshUI();
            }
        }

        private async Task ReconcileCurrentFolderContents()
        {
            var changed = await metadataManager.Resync();
            if (changed)
            {
                await LoadMetadata();
            }
        }

        public override async Task Initialize()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
            await LoadMetadata();
        }

        public async Task LoadMetadata()
        {
            var metadata = await metadataManager.VideoRepo.GetAllMetadata();
            metadataEntries.Clear();
            foreach (var entry in metadata.OrderBy(x => RemoveThe(x.Artist)).ThenBy(x => x.Title))
            {
                metadataEntries.Add(new VideoMetadataViewModel(entry));
            }
            FilterMetadataEntries();
        }

        private string RemoveThe(string input)
        {
            const string prefix = "the ";
            if (input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return input.Substring(prefix.Length);
            }
            return input;
        }

        public void SortItems(string sortMemberPath, bool ascending)
        {
            IOrderedEnumerable<VideoMetadataViewModel> sortedEntries = sortMemberPath switch
            {
                "Status" => ascending ? FilteredMetadataEntries.OrderBy(x => x.Status) : FilteredMetadataEntries.OrderByDescending(x => x.Status),
                "Artist" => ascending ? FilteredMetadataEntries.OrderBy(x => x.Artist) : FilteredMetadataEntries.OrderByDescending(x => x.Artist),
                "Title" => ascending ? FilteredMetadataEntries.OrderBy(x => x.Title) : FilteredMetadataEntries.OrderByDescending(x => x.Title),
                "Album" => ascending ? FilteredMetadataEntries.OrderBy(x => x.Album) : FilteredMetadataEntries.OrderByDescending(x => x.Album),
                "Year" => ascending ? FilteredMetadataEntries.OrderBy(x => x.ReleaseYear) : FilteredMetadataEntries.OrderByDescending(x => x.ReleaseYear),
                _ => throw new Exception("Invalid field")
            };

            if (sortedEntries is null)
                return;

            if (sortMemberPath != "Artist")
                sortedEntries = sortedEntries.ThenBy(x => x.Artist);
            else
                sortedEntries = sortedEntries.ThenBy(x => x.Title);

            // Reorder the collection in place
            var sortedList = sortedEntries.ToList();
            metadataEntries = [..sortedList];
            FilterMetadataEntries();
        }

    }
}