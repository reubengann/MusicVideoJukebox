using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MetadataEntry : BaseViewModel
    {
        bool settingProgrammatically = false;

        public int VideoId { get => videoMetadata.VideoId; }
        public string Filename { get => videoMetadata.Filename; }
        public string Title { get => videoMetadata.Title; set => SetPropertyForMetadata(videoMetadata.Title, value, v => videoMetadata.Title = v); }
        public string Artist { get => videoMetadata.Artist; set => SetPropertyForMetadata(videoMetadata.Artist, value, v => videoMetadata.Artist = v); }
        public string? Album { get => videoMetadata.Album; set => SetPropertyForMetadata(videoMetadata.Album, value, v => videoMetadata.Album = v); }
        public int? Year { get => videoMetadata.ReleaseYear; set => SetPropertyForMetadata(videoMetadata.ReleaseYear, value, v => videoMetadata.ReleaseYear = v); }

        private bool SetPropertyForMetadata<T>(T currentValue, T newValue, Action<T> setValue, [CallerMemberName] string propertyName = null!)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            setValue(newValue); // Update the value in the wrapped object
            OnPropertyChanged(propertyName); // Notify UI
            MaybeTouch();
            return true;
        }

        public MetadataEntry(VideoMetadata videoMetadata)
        {
            this.videoMetadata = videoMetadata;
        }

        private MetadataStatus _status = MetadataStatus.NotDone;
        private readonly VideoMetadata videoMetadata;

        public bool IsModified { get; private set; } = false;
        public void StartProgrammaticUpdate() { settingProgrammatically = true; Status = MetadataStatus.Fetching; }
        public void EndProgrammaticUpdate() { settingProgrammatically = false; }

        private void MaybeTouch()
        {
            IsModified = true;
            if (settingProgrammatically)
            {
                Status = MetadataStatus.Done;
            }
            else
            {
                Status = MetadataStatus.Manual;
            }
        }

        public MetadataStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public VideoMetadata MetadataObject => videoMetadata;

        public void Reset()
        {
            IsModified = false;
        }
    }

    public class MetadataEditViewModel : AsyncInitializeableViewModel
    {
        public ObservableCollection<MetadataEntry> MetadataEntries { get; } = new ObservableCollection<MetadataEntry>();

        public ICommand FetchMetadataCommand { get; }
        public ICommand RefreshDatabaseCommand { get; }
        public ICommand SaveChangesCommand { get; }

        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;
        private readonly IDialogService dialogService;
        private IMetadataManager metadataManager = null!;

        public MetadataEditViewModel(IMetadataManagerFactory metadataManagerFactory, LibraryStore libraryStore, IDialogService dialogService)
        {
            FetchMetadataCommand = new DelegateCommand(async () => await FetchMetadataFromWeb());
            RefreshDatabaseCommand = new DelegateCommand(async () => await RefreshDatabase());
            SaveChangesCommand = CreateSafeAsyncCommand(SaveChanges);
            this.metadataManagerFactory = metadataManagerFactory;
            this.libraryStore = libraryStore;
            this.dialogService = dialogService;
        }

        private async Task SaveChanges()
        {
            foreach (var entry in MetadataEntries)
            {
                if(entry.IsModified)
                {
                    await metadataManager.UpdateVideoMetadata(entry.MetadataObject);
                    entry.Reset();
                }
            }
        }

        private async Task FetchMetadataFromWeb()
        {
            //foreach (var entry in MetadataEntries)
            //{
            //    var updatedMetadata = await _webMetadataService.FetchMetadataAsync(entry.Filename);
            //    if (updatedMetadata != null)
            //    {
            //        entry.Title = updatedMetadata.Title;
            //        entry.Artist = updatedMetadata.Artist;
            //        entry.Album = updatedMetadata.Album;
            //        entry.Year = updatedMetadata.Year;
            //    }
            //}
        }

        private async Task RefreshDatabase()
        {
            //var newEntries = await _metadataRepository.RefreshDatabaseAsync();
            //foreach (var entry in newEntries)
            //{
            //    MetadataEntries.Add(entry);
            //}
        }

        public override async Task Initialize()
        {
            if (libraryStore.FolderPath == null) return;
            metadataManager =  metadataManagerFactory.Create(libraryStore.FolderPath);
            await LoadMetadata();
        }

        public async Task LoadMetadata()
        {
            var metadata = await metadataManager.GetAllMetadata();
            MetadataEntries.Clear();
            foreach (var entry in metadata)
            {
                MetadataEntries.Add(new MetadataEntry(entry));               
            }
        }

        private DelegateCommand CreateSafeAsyncCommand(Func<Task> execute)
        {
            return new DelegateCommand(async () =>
            {
                try
                {
                    await execute();
                }
                catch (Exception ex)
                {
                    dialogService.ShowError($"Could not save: {ex.Message}");
                    throw;
                }
            });
        }
    }
}