
namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataManager : IMetadataManager
    {
        private readonly string filepath;
        private readonly string folderPath;
        private readonly IVideoRepo videoRepo;
        private readonly IFileSystemService fileSystemService;

        public MetadataManager(string folderPath, IVideoRepo videoRepo, IFileSystemService fileSystemService)
        {
            this.folderPath = folderPath;
            filepath = Path.Combine(folderPath, "meta.db");
            this.videoRepo = videoRepo;
            this.fileSystemService = fileSystemService;
        }

        public async Task EnsureCreated()
        {
            if (fileSystemService.FileExists(filepath))
            {
                return;
            }
            await videoRepo.CreateTables();
            var rows = fileSystemService.ListMp4Files(folderPath);
            List<BasicInfo> infos = [];
            foreach (var row in rows)
            {
                var (artist, title) = FileNameHelpers.ParseFileNameIntoArtistTitle(row);
                infos.Add(new BasicInfo { Artist = artist, Title = title, Filename = row });
            }
            await videoRepo.AddBasicInfos(infos);
        }
    }
}
