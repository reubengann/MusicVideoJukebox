using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeMetadataManager : IMetadataManager
    {
        public List<string> ExistingMetadataFolders { get; internal set; } = [];

        public bool CreateMetadata(string folderPath)
        {
            ExistingMetadataFolders.Add(folderPath);
            return true;
        }

        public bool HasMetadata(string folderPath)
        {
            return ExistingMetadataFolders.Contains(folderPath);
        }
    }
}