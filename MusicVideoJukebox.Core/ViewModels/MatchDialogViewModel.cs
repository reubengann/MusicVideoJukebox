using MusicVideoJukebox.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class MatchDialogViewModel
    {
        private readonly VideoMetadata metadata;
        private readonly IMetadataManager metadataManager;

        public bool SelectedPressed { get; set; }
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public ObservableCollection<ScoredMetadata> Candidates { get; set; } = [];

        public MatchDialogViewModel(VideoMetadata metadata, IMetadataManager metadataManager)
        {
            this.metadata = metadata;
            this.metadataManager = metadataManager;
        }

        public async Task Initialize()
        {
            var candidates = await metadataManager.GetScoredCandidates(metadata.Artist, metadata.Title);
            foreach (var candidate in candidates.OrderByDescending(x => x.Similarity))
            {
                Candidates.Add(candidate);
            }
        }
    }
}
