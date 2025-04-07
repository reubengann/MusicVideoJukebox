namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataManager(string folderPath, IVideoRepo videoRepo, IFileSystemService fileSystemService, IReferenceDataRepo referenceDataRepo) : IMetadataManager
    {
        private readonly string folderPath = folderPath;
        private readonly IVideoRepo videoRepo = videoRepo;
        private readonly IFileSystemService fileSystemService = fileSystemService;
        private readonly IReferenceDataRepo referenceDataRepo = referenceDataRepo;

        public IVideoRepo VideoRepo => videoRepo;

        public async Task EnsureCreated()
        {
            if (await videoRepo.IsDatabaseInitialized())
            {
                return;
            }
            await videoRepo.InitializeDatabase();
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

        public async Task<List<PlaylistTrackForViewmodel>> GetPlaylistTracksForViewmodel(int playlistId)
        {
            var tracks = await videoRepo.GetPlaylistTracks(playlistId);
            return tracks.Select(x => new PlaylistTrackForViewmodel { Artist =  x.Artist, Title = x.Title, PlaylistId = playlistId, PlaylistVideoId = x.PlaylistVideoId, PlayOrder = x.PlayOrder, VideoId = x.VideoId }).ToList();
        }

        public async Task<List<PlaylistTrackForViewmodel>> ShuffleTracks(int playlistId)
        {
            var tracks = await GetPlaylistTracksForViewmodel(playlistId);
            var shuffledTracks = tracks.OrderBy(_ => Guid.NewGuid()).ToList();

            int order = 1;
            var updates = new List<(int playlistId, int videoId, int order)>();
            foreach (var track in shuffledTracks)
            {
                updates.Add((playlistId, track.VideoId, order));
                track.PlayOrder = order;
                order++;
            }
            await VideoRepo.UpdatePlaylistTrackOrderBatch(updates);
            await VideoRepo.UpdatePlayStatus(playlistId, 1);
            return shuffledTracks;
        }

        public Task<List<SearchResult>> SearchReferenceDb(string queryString)
        {
            return referenceDataRepo.SearchReferenceDb(queryString);
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
                return (artist.Trim(), title.Trim()); // sometimes there are extra spaces between the name and the hyphen
            }
            else
            {
                return ("Unknown", nameOnly);
            }
        }
    }
}
