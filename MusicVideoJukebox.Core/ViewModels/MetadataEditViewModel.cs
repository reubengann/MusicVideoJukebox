using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MetadataEntry : BaseViewModel
    {
        required public int VideoId { get; set; }
        required public string Filename { get; set; }
        required public string Title { get; set; }
        required public string Artist { get; set; }
        public string? Album { get; set; }
        public int? Year { get; set; }

        private MetadataStatus _status = MetadataStatus.NotDone;
        public MetadataStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }
    }

    public class MetadataEditViewModel : AsyncInitializeableViewModel
    {
        public ObservableCollection<MetadataEntry> MetadataEntries { get; } = new ObservableCollection<MetadataEntry>();

        public DelegateCommand FetchMetadataCommand { get; }
        public DelegateCommand RefreshDatabaseCommand { get; }

        private readonly IVideoRepo _metadataRepository;
        private readonly IMetadataProvider _webMetadataService;

        public MetadataEditViewModel(IVideoRepo metadataRepository, IMetadataProvider webMetadataService)
        {
            _metadataRepository = metadataRepository;
            _webMetadataService = webMetadataService;

            FetchMetadataCommand = new DelegateCommand(async () => await FetchMetadataFromWeb());
            RefreshDatabaseCommand = new DelegateCommand(async () => await RefreshDatabase());
        }

        public async Task LoadMetadata()
        {
            var metadata = await _metadataRepository.GetAllMetadata();
            MetadataEntries.Clear();
            foreach (var entry in metadata)
            {
                MetadataEntries.Add(new MetadataEntry {
                    VideoId = entry.VideoId, 
                    Filename = entry.Filename, 
                    Year = entry.ReleaseYear,
                    Title = entry.Title, 
                    Album = entry.Album, 
                    Artist = entry.Artist, 
                    Status = entry.Status,
                });
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
            await LoadMetadata();
        }
    }
}