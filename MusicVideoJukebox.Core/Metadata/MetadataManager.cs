using FuzzySharp.PreProcess;
using FuzzySharp;
using System.Reflection.Metadata;

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
                infos.Add(FileNameToBasicInfo(row));
            }
            await videoRepo.AddBasicInfos(infos);
        }

        private static BasicInfo FileNameToBasicInfo(string filename)
        {
            var (artist, title) = FileNameHelpers.ParseFileNameIntoArtistTitle(filename);
            return new BasicInfo { Artist = artist, Title = title, Filename = filename };
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

        public async Task<bool> Resync()
        {
            var anyChanges = false;
            var filesOnDisk = fileSystemService.ListMp4Files(folderPath).ToHashSet();
            var metadataInDb = await videoRepo.GetAllMetadata();
            var filesInDb = metadataInDb.Select(x => x.Filename).ToHashSet();
            var toAdd = filesOnDisk.Except(filesInDb);
            
            if (toAdd.Any())
            {
                await videoRepo.AddBasicInfos(toAdd.Select(x => FileNameToBasicInfo(x)).ToList());
                anyChanges = true;
            }
            var toRemove = metadataInDb.Where(x => !filesOnDisk.Contains(x.Filename)).Select(x => x.VideoId).ToList();
            if (toRemove.Any())
            {
                foreach (var id in toRemove)
                {
                    await videoRepo.RemoveMetadata(id);
                }
                anyChanges = true;
            }
            return anyChanges;
        }

        public Task<List<Playlist>> GetPlaylists()
        {
            return videoRepo.GetPlaylists();
        }
    }
}
