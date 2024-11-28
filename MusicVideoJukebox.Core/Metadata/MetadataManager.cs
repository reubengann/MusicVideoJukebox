
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IMetadataManager
    {
        Task<bool> EnsureCreated();
    }

    public interface IVideoRepoFactory
    {
        IVideoRepo Create(string folderPath);
    }

    public interface IVideoRepo
    {
        Task CreateTables();
    }

    public interface IMetadataManagerFactory
    {
        IMetadataManager Create(string folderPath);
    }

    public class VideoRepo : IVideoRepo
    {
        public Task CreateTables()
        {
            throw new NotImplementedException();
        }
    }

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

    public class MetadataManager : IMetadataManager
    {
        private readonly string filepath;
        private readonly IVideoRepo videoRepo;
        private readonly IFileSystemService fileSystemService;

        public MetadataManager(string folderPath, IVideoRepo videoRepo, IFileSystemService fileSystemService)
        {
            this.filepath = Path.Combine(folderPath, "meta.db");
            this.videoRepo = videoRepo;
            this.fileSystemService = fileSystemService;
        }

        public async Task<bool> EnsureCreated()
        {
            if (fileSystemService.FileExists(filepath))
            {
                return true;
            }
            await videoRepo.CreateTables();
            return true;
        }
    }
}
