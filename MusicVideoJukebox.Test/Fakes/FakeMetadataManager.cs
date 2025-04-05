using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataManager : IMetadataManager
    {
        public string folderPath;

        public List<string> CreatedMetadataFolders { get; internal set; } = [];
        

        public IVideoRepo VideoRepo => videoRepo;
        public FakeVideoRepo ConcreteVideoRepo => videoRepo;

        public int SearchCount = 0;
        public List<ScoredMetadata> ScoredCandidates = [];
        public bool WasShuffled = false;
        public Dictionary<string, GetAlbumYearResult> ReferenceDataToGet = [];
        public bool SayChangesWereMade = false;

        private FakeVideoRepo videoRepo = new();

        

        public FakeMetadataManager(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public async Task EnsureCreated()
        {
            await Task.CompletedTask;
            CreatedMetadataFolders.Add(folderPath);
        }

        public async Task<GetAlbumYearResult> TryGetAlbumYear(string artist, string track)
        {
            await Task.CompletedTask;
            if (ReferenceDataToGet.ContainsKey($"{artist} {track}"))
            {
                return ReferenceDataToGet[$"{artist} {track}"];
            }
            return new GetAlbumYearResult { Success = false };
        }

        public Task<bool> Resync()
        {
            return Task.FromResult(SayChangesWereMade);
        }

        public async Task<List<PlaylistTrackForViewmodel>> GetPlaylistTracksForViewmodel(int playlistId)
        {
            await Task.CompletedTask;
            int i = 0;
            return (await VideoRepo.GetPlaylistTracks(playlistId)).Select(x => new PlaylistTrackForViewmodel { Artist = x.Artist, Title = x.Title, PlayOrder = i++ }).ToList();
        }

        public Task<List<PlaylistTrackForViewmodel>> ShuffleTracks(int playlistId)
        {
            WasShuffled = true;
            return Task.FromResult(videoRepo.MetadataEntries.Select(x => new PlaylistTrackForViewmodel { Artist = x.Artist, Title = x.Title }).ToList());
        }

        public Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ScoredMetadata>> GetScoredCandidates(string artist, string track)
        {
            SearchCount++;
            await Task.CompletedTask;
            return ScoredCandidates;
        }

        public Task UpdateAnalysisResult(VideoAnalysisEntry entry)
        {
            throw new NotImplementedException();
        }
    }
}