
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

        public MetadataManagerFactory(IVideoRepoFactory videoRepoFactory)
        {
            this.videoRepoFactory = videoRepoFactory;
        }

        public IMetadataManager Create(string folderPath)
        {
            return new MetadataManager(folderPath, videoRepoFactory.Create(folderPath));
        }
    }

    public class MetadataManager : IMetadataManager
    {
        private readonly string folderPath;
        private readonly IVideoRepo videoRepo;

        public MetadataManager(string folderPath, IVideoRepo videoRepo)
        {
            this.folderPath = folderPath;
            this.videoRepo = videoRepo;
        }

        public Task<bool> EnsureCreated()
        {
            throw new NotImplementedException();
        }
    }
}
