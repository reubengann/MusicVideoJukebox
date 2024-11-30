using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeVideoRepo : IVideoRepo
    {
        public bool TablesCreated = false;
        public List<VideoMetadata> MetadataEntries = [];
        public List<VideoMetadata> UpdatedEntries = [];

        public IEnumerable<char>? FolderPath { get; internal set; }
        public List<BasicInfo> BasicRowsCreated { get; internal set; } = [];

        public Task AddBasicInfos(List<BasicInfo> basicInfos)
        {
            BasicRowsCreated.AddRange(basicInfos);
            return Task.CompletedTask;
        }

        public async Task CreateTables()
        {
            await Task.CompletedTask;
            TablesCreated = true;
        }

        public async Task<List<VideoMetadata>> GetAllMetadata()
        {
            await Task.CompletedTask;
            return MetadataEntries;
        }

        public Task UpdateMetadata(VideoMetadata metadata)
        {
            UpdatedEntries.Add(metadata);
            return Task.CompletedTask;
        }
    }
}