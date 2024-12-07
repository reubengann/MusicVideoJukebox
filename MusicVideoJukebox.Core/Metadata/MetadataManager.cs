using FuzzySharp.PreProcess;
using FuzzySharp;

namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataManager : IMetadataManager
    {
        private readonly string filepath;
        private readonly string folderPath;
        private readonly IVideoRepo videoRepo;
        private readonly IFileSystemService fileSystemService;
        private readonly IReferenceDataRepo referenceDataRepo;

        const int similarityThreshold = 80;

        public MetadataManager(string folderPath, IVideoRepo videoRepo, IFileSystemService fileSystemService, IReferenceDataRepo referenceDataRepo)
        {
            this.folderPath = folderPath;
            filepath = Path.Combine(folderPath, "meta.db");
            this.videoRepo = videoRepo;
            this.fileSystemService = fileSystemService;
            this.referenceDataRepo = referenceDataRepo;
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

        public async Task<List<VideoMetadata>> GetAllMetadata()
        {
            return await videoRepo.GetAllMetadata();
        }

        public async Task<GetAlbumYearResult> TryGetAlbumYear(string artist, string track)
        {
            var maybeExactResult = await referenceDataRepo.TryGetExactMatch(artist, track);
            if (maybeExactResult.Success)
            {
                ArgumentNullException.ThrowIfNull(maybeExactResult.FetchedMetadata);
                return new GetAlbumYearResult { Success = true, AlbumTitle = maybeExactResult.FetchedMetadata.AlbumTitle, ReleaseYear = maybeExactResult.FetchedMetadata.FirstReleaseDateYear };
            }
            // fuzzy match
            var partialMatches = await referenceDataRepo.GetCandidates(artist, track);
            var bestMatch = partialMatches
            .Select(candidate => new
            {
                Candidate = candidate,
                Similarity = GetSimilarity($"{artist} {track}", $"{candidate.ArtistName} {candidate.TrackName}")
            })
            .Where(x => x.Similarity >= similarityThreshold)
            .OrderByDescending(x => x.Similarity)
            .FirstOrDefault();
            if (bestMatch != null)
            {
                return new GetAlbumYearResult { Success = true, AlbumTitle = bestMatch.Candidate.AlbumTitle, ReleaseYear = bestMatch.Candidate.FirstReleaseDateYear };
            }
            return new GetAlbumYearResult { Success = false };
        }

        public async Task UpdateVideoMetadata(VideoMetadata entry)
        {
            await videoRepo.UpdateMetadata(entry);
        }

        private int GetSimilarity(string target, string item)
        {
            int similarity = Fuzz.WeightedRatio(target, item, PreprocessMode.Full);
            return similarity;
        }
    }
}
