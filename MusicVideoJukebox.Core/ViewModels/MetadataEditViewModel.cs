using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MetadataEntryViewModel : BaseViewModel
    {
        public int VideoId { get => videoMetadata.VideoId; }
        public string Filename { get => videoMetadata.Filename; }
        public string Title { get => videoMetadata.Title; set => SetUnderlyingProperty(videoMetadata.Title, value, v => videoMetadata.Title = v); }
        public string Artist { get => videoMetadata.Artist; set => SetUnderlyingProperty(videoMetadata.Artist, value, v => videoMetadata.Artist = v); }
        public string? Album { get => videoMetadata.Album; set => SetUnderlyingProperty(videoMetadata.Album, value, v => videoMetadata.Album = v); }
        public int? Year { get => videoMetadata.ReleaseYear; set => SetUnderlyingProperty(videoMetadata.ReleaseYear, value, v => videoMetadata.ReleaseYear = v); }
        public MetadataStatus Status
        {
            get => videoMetadata.Status;
            set { videoMetadata.Status = value; SetUnderlyingProperty(videoMetadata.Status, value, v => videoMetadata.Status = v); }
        }

        public MetadataEntryViewModel(VideoMetadata videoMetadata)
        {
            this.videoMetadata = videoMetadata;
        }

        private readonly VideoMetadata videoMetadata;

        public VideoMetadata MetadataObject => videoMetadata;
    }

    public class MetadataEditViewModel : AsyncInitializeableViewModel
    {
        public ObservableCollection<MetadataEntryViewModel> MetadataEntries { get; } = new ObservableCollection<MetadataEntryViewModel>();

        public bool IsBusy { get => isBusy; set { isBusy = value; SetProperty(ref isBusy, value); } }

        public ICommand EditMetadataCommand { get; }
        public ICommand RefreshDatabaseCommand { get; }

        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;
        private readonly IDialogService dialogService;
        private IMetadataManager metadataManager = null!;
        private bool isBusy;
        private MetadataEntryViewModel? selectedItem;

        public MetadataEntryViewModel? SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public MetadataEditViewModel(IMetadataManagerFactory metadataManagerFactory, LibraryStore libraryStore, IDialogService dialogService)
        {
            EditMetadataCommand = new DelegateCommand(LaunchMatchDialog);
            RefreshDatabaseCommand = new DelegateCommand(async () => await ReconcileCurrentFolderContents());
            this.metadataManagerFactory = metadataManagerFactory;
            this.libraryStore = libraryStore;
            this.dialogService = dialogService;
        }

        private void LaunchMatchDialog()
        {
            if (SelectedItem == null) return;
            var result = dialogService.ShowMatchDialog(new MatchDialogViewModel(SelectedItem.MetadataObject, metadataManager));
            if (result.Accepted)
            {
                ArgumentNullException.ThrowIfNull(result.ScoredMetadata);
                var scoredMetadata = result.ScoredMetadata;
                // I think technically this isn't totally needed. The viewmodel does need to be told to update, but I don't think
                // each field needs to be set because we passed a reference to the constructor.
                SelectedItem.Artist = scoredMetadata.Artist;
                SelectedItem.Album = scoredMetadata.Album;
                SelectedItem.Title = scoredMetadata.Title;
                SelectedItem.Year = scoredMetadata.ReleaseYear;
                SelectedItem.Status = MetadataStatus.Done;
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
            MetadataEntries.Clear();
            foreach (var entry in metadata)
            {
                MetadataEntries.Add(new MetadataEntryViewModel(entry));
            }
        }
    }
}