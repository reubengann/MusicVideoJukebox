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
                infos.Add(FileNameToBasicInfo(row));
            }
            await videoRepo.AddBasicInfos(infos);
            var added = await videoRepo.GetAllMetadata();
            foreach (var row in added)
            {
                await videoRepo.AppendSongToPlaylist(1, row.VideoId);
            }
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
            if (toRemove.Count != 0)
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

        public async Task<int> SavePlaylist(Playlist playlist)
        {
            return await videoRepo.SavePlaylist(playlist);
        }

        public async Task UpdatePlaylistName(int id, string name)
        {
            await videoRepo.UpdatePlaylistName(id, name);
        }

        public async Task<List<PlaylistTrackForViewmodel>> GetPlaylistTracksForViewmodel(int playlistId)
        {
            var tracks = await videoRepo.GetPlaylistTracks(playlistId);
            return tracks.Select(x => new PlaylistTrackForViewmodel { Artist =  x.Artist, Title = x.Title, PlaylistId = playlistId, PlaylistVideoId = x.PlaylistVideoId, PlayOrder = x.PlayOrder, VideoId = x.VideoId }).ToList();
        }

        public async Task<List<PlaylistTrack>> GetPlaylistTracks(int playlistId)
        {
            return await videoRepo.GetPlaylistTracks(playlistId);
        }

        public async Task AppendSongToPlaylist(int playlistId, int videoId)
        {
            await videoRepo.AppendSongToPlaylist(playlistId, videoId);
        }

        public async Task<List<PlaylistTrackForViewmodel>> ShuffleTracks(int playlistId)
        {
            var tracks = await GetPlaylistTracksForViewmodel(playlistId);
            var shuffledTracks = tracks.OrderBy(_ => Guid.NewGuid()).ToList();

            int order = 1;
            foreach (var track in shuffledTracks)
            {
                await UpdatePlaylistTrackOrder(playlistId, track.VideoId, order);
                track.PlayOrder = order;
                order++;
            }
            return shuffledTracks;
        }

        public async Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order)
        {
            await videoRepo.UpdatePlaylistTrackOrder(playlistId, videoId, order);
        }
    }
}
