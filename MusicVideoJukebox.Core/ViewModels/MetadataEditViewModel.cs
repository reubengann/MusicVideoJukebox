using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MetadataEntryViewModel : BaseViewModel
    {
        bool settingProgrammatically = false;

        public int VideoId { get => videoMetadata.VideoId; }
        public string Filename { get => videoMetadata.Filename; }
        public string Title { get => videoMetadata.Title; set => SetPropertyForMetadata(videoMetadata.Title, value, v => videoMetadata.Title = v); }
        public string Artist { get => videoMetadata.Artist; set => SetPropertyForMetadata(videoMetadata.Artist, value, v => videoMetadata.Artist = v); }
        public string? Album { get => videoMetadata.Album; set => SetPropertyForMetadata(videoMetadata.Album, value, v => videoMetadata.Album = v); }
        public int? Year { get => videoMetadata.ReleaseYear; set => SetPropertyForMetadata(videoMetadata.ReleaseYear, value, v => videoMetadata.ReleaseYear = v); }
        public MetadataStatus Status
        {
            get => videoMetadata.Status;
            set { _status = value; SetPropertyForMetadata(videoMetadata.Status, value, v => videoMetadata.Status = v); }
        }

        private bool SetPropertyForMetadata<T>(T currentValue, T newValue, Action<T> setValue, [CallerMemberName] string propertyName = null!)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            setValue(newValue); // Update the value in the wrapped object
            OnPropertyChanged(propertyName); // Notify UI
            MaybeTouch();
            return true;
        }

        public MetadataEntryViewModel(VideoMetadata videoMetadata)
        {
            this.videoMetadata = videoMetadata;
        }

        private MetadataStatus _status = MetadataStatus.NotDone;
        private readonly VideoMetadata videoMetadata;
        
        private bool isModified = false;

        public bool IsModified { get => isModified; private set => SetProperty(ref isModified, value); }
        
        public void StartProgrammaticUpdate() { settingProgrammatically = true; Status = MetadataStatus.Fetching; }
        public void EndProgrammaticUpdate() { settingProgrammatically = false; }

        private void MaybeTouch()
        {
            IsModified = true;
            if (!settingProgrammatically)
            {
                Status = MetadataStatus.Manual;
            }
        }

        public VideoMetadata MetadataObject => videoMetadata;

        public void Reset()
        {
            IsModified = false;
        }
    }

    public class MetadataEditViewModel : AsyncInitializeableViewModel
    {
        public ObservableCollection<MetadataEntryViewModel> MetadataEntries { get; } = new ObservableCollection<MetadataEntryViewModel>();

        public bool IsBusy { get => isBusy; set { isBusy = value; SetProperty(ref isBusy, value); } }

        public ICommand FetchMetadataCommand { get; }
        public ICommand RefreshDatabaseCommand { get; }
        public DelegateCommand SaveChangesCommand { get; }

        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;
        private readonly IDialogService dialogService;
        private IMetadataManager metadataManager = null!;
        private bool isBusy;

        public MetadataEditViewModel(IMetadataManagerFactory metadataManagerFactory, LibraryStore libraryStore, IDialogService dialogService)
        {
            FetchMetadataCommand = new DelegateCommand(async () => await FetchMetadata());
            RefreshDatabaseCommand = new DelegateCommand(async () => await ReconcileCurrentFolderContents());
            SaveChangesCommand = CreateSafeAsyncCommand(SaveChanges, CanSaveChanges);
            this.metadataManagerFactory = metadataManagerFactory;
            this.libraryStore = libraryStore;
            this.dialogService = dialogService;
            MetadataEntries.CollectionChanged += MetadataEntries_CollectionChanged;
        }

        private bool CanSaveChanges()
        {
            return MetadataEntries.Any(entry => entry.IsModified);
        }

        private void MetadataEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MetadataEntryViewModel newItem in e.NewItems)
                {
                    newItem.PropertyChanged += MetadataEntry_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (MetadataEntryViewModel oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= MetadataEntry_PropertyChanged;
                }
            }
        }

        private void MetadataEntry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MetadataEntryViewModel.IsModified))
            {
                SaveChangesCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task SaveChanges()
        {
            IsBusy = true;
            try
            {
                foreach (var entry in MetadataEntries)
                {
                    if (entry.IsModified)
                    {
                        await metadataManager.UpdateVideoMetadata(entry.MetadataObject);
                        entry.Reset();
                    }
                }
            }
            catch(Exception ex)
            {
                dialogService.ShowError($"Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FetchMetadata()
        {
            foreach (var entry in MetadataEntries)
            {
                if (entry.Status != MetadataStatus.NotDone) continue;
                entry.StartProgrammaticUpdate();
                var result = await metadataManager.TryGetAlbumYear(entry.Artist, entry.Title);
                if (result.Success)
                {
                    //entry.Status = MetadataStatus.Done;
                    entry.Album = result.AlbumTitle;
                    entry.Year = result.ReleaseYear;
                    entry.Status = MetadataStatus.Done;
                }
                else
                {
                    entry.Status = MetadataStatus.NotFound;
                }
                entry.EndProgrammaticUpdate();
            }
            OnPropertyChanged(); // force an update to enable the save button
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
            metadataManager =  metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
            await LoadMetadata();
        }

        public async Task LoadMetadata()
        {
            var metadata = await metadataManager.GetAllMetadata();
            MetadataEntries.Clear();
            foreach (var entry in metadata)
            {
                MetadataEntries.Add(new MetadataEntryViewModel(entry));               
            }
        }

        private DelegateCommand CreateSafeAsyncCommand(Func<Task> execute, Func<bool> canExecute = null!)
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
            },
            canExecute ?? (() => true)
            );
        }
    }
}