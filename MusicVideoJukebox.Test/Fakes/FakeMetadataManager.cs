using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataManager : IMetadataManager
    {
        public string folderPath;

        public List<string> CreatedMetadataFolders { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntries { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntriesUpdated { get; internal set; } = [];
        public Dictionary<string, GetAlbumYearResult> ReferenceDataToGet = [];
        public bool SayChangesWereMade = false;

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

        public async Task<GetAlbumYearResult> TryGetAlbumYear(string artist, string track)
        {
            await Task.CompletedTask;
            if (ReferenceDataToGet.ContainsKey($"{artist} {track}"))
            {
                return ReferenceDataToGet[$"{artist} {track}"];
            }
            return new GetAlbumYearResult { Success = false };
        }

        public Task<bool> Resync()
        {
            return Task.FromResult(SayChangesWereMade);
        }
    }
}