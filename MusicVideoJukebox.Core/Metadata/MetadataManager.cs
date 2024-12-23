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
            var bestMatch = (await GetScoredCandidates(artist, track))
            .Where(x => x.Similarity >= similarityThreshold)
            .OrderByDescending(x => x.Similarity)
            .FirstOrDefault();
            if (bestMatch != null)
            {
                return new GetAlbumYearResult { Success = true, AlbumTitle = bestMatch.AlbumTitle, ReleaseYear = bestMatch.FirstReleaseDateYear };
            }
            return new GetAlbumYearResult { Success = false };
        }

        public async Task<List<ScoredMetadata>> GetScoredCandidates(string artist, string track)
        {
            var partialMatches = await referenceDataRepo.GetCandidates(artist, track);
            return partialMatches.Select(x => new ScoredMetadata { 
                AlbumTitle = x.AlbumTitle, 
                ArtistName = x.ArtistName, 
                FirstReleaseDateYear = x.FirstReleaseDateYear,
                TrackName = x.TrackName,
                Similarity = GetSimilarity($"{artist} {track}", $"{x.ArtistName} {x.TrackName}")
            }).ToList();
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

        public async Task<int> AppendSongToPlaylist(int playlistId, int videoId)
        {
            return await videoRepo.AppendSongToPlaylist(playlistId, videoId);
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

        public async Task InsertAnalysisResult(VideoAnalysisEntry entry)
        {
            await videoRepo.InsertAnalysisResult(entry);
        }

        public async Task<List<VideoAnalysisEntry>> GetAnalysisResults()
        {
            return await videoRepo.GetAnalysisResults();
        }
    }

    public static class FileNameHelpers
    {
        public static (string, string) ParseFileNameIntoArtistTitle(string filename)
        {
            var nameOnly = Path.GetFileNameWithoutExtension(filename);
            if (nameOnly.Contains(" - "))
            {
                var parts = nameOnly.Split(" - ");
                string title;
                if (parts.Length > 2)
                {
                    title = string.Join(" - ", parts.Skip(1));
                }
                else
                {
                    title = parts[1];
                }
                string artist = parts[0];
                return (artist, title);
            }
            else
            {
                return ("Unknown", nameOnly);
            }
        }
    }
}
