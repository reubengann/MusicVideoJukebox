using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataManager : IMetadataManager
    {
        public string folderPath;

        public List<string> CreatedMetadataFolders { get; internal set; } = [];

        public FakeMetadataManager(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public async Task<bool> EnsureCreated()
        {
            await Task.CompletedTask;
            CreatedMetadataFolders.Add(folderPath);
            return true;
        }
    }
}