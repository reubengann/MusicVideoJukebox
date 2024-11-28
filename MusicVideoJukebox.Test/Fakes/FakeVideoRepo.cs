using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeVideoRepo : IVideoRepo
    {
        public bool TablesCreated = false;

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
    }
}