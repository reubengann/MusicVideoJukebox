
namespace MusicVideoJukebox.Core.Metadata
{
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
