using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeVideoRepo : IVideoRepo
    {
        public bool TablesCreated = false;

        public IEnumerable<char>? FolderPath { get; internal set; }

        public Task AddBasicInfos(List<BasicInfo> basicInfos)
        {
            throw new NotImplementedException();
        }

        public async Task CreateTables()
        {
            await Task.CompletedTask;
            TablesCreated = true;
        }
    }
}