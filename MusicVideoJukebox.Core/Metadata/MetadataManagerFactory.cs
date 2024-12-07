
namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataManagerFactory : IMetadataManagerFactory
    {
        private readonly IVideoRepoFactory videoRepoFactory;
        private readonly IFileSystemService fileSystemService;
        private readonly IReferenceDataRepo referenceDataRepo;

        public MetadataManagerFactory(IVideoRepoFactory videoRepoFactory, IFileSystemService fileSystemService, IReferenceDataRepo referenceDataRepo)
        {
            this.videoRepoFactory = videoRepoFactory;
            this.fileSystemService = fileSystemService;
            this.referenceDataRepo = referenceDataRepo;
        }

        public IMetadataManager Create(string folderPath)
        {
            return new MetadataManager(folderPath, videoRepoFactory.Create(folderPath), fileSystemService, referenceDataRepo);
        }
    }
}
