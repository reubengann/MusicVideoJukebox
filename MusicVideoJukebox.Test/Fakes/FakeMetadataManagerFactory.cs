using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataManagerFactory : IMetadataManagerFactory
    {
        public FakeMetadataManager ToReturn = new FakeMetadataManager("");

        public IMetadataManager Create(string folderPath)
        {
            ToReturn.folderPath = folderPath;
            return ToReturn;
        }
    }
}