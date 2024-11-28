
namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataManagerFactory : IMetadataManagerFactory
    {
        private readonly IVideoRepoFactory videoRepoFactory;
        private readonly IFileSystemService fileSystemService;

        public MetadataManagerFactory(IVideoRepoFactory videoRepoFactory, IFileSystemService fileSystemService)
        {
            this.videoRepoFactory = videoRepoFactory;
            this.fileSystemService = fileSystemService;
        }

        public IMetadataManager Create(string folderPath)
        {
            return new MetadataManager(folderPath, videoRepoFactory.Create(folderPath), fileSystemService);
        }
    }
}
