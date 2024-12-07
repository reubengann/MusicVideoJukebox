using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataManager : IMetadataManager
    {
        public string folderPath;

        public List<string> CreatedMetadataFolders { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntries { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntriesUpdated { get; internal set; } = [];

        public FakeMetadataManager(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public async Task EnsureCreated()
        {
            await Task.CompletedTask;
            CreatedMetadataFolders.Add(folderPath);
        }

        public async Task<List<VideoMetadata>> GetAllMetadata()
        {
            await Task.CompletedTask;
            return MetadataEntries;
        }

        public Task UpdateVideoMetadata(VideoMetadata entry)
        {
            MetadataEntriesUpdated.Add(entry);
            return Task.CompletedTask;
        }

        public Task<GetAlbumYearResult> TryGetAlbumYear(string artist, string track)
        {
            throw new NotImplementedException();
        }
    }
}