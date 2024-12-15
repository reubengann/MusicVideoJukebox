using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class AvailableTrackViewModel : BaseViewModel
    {
        private readonly VideoMetadata meta;
        public bool IsModified { get; set; } = false;

        public AvailableTrackViewModel(VideoMetadata meta)
        {
            this.meta = meta;
        }

        public string Name => $"{meta.Artist} - {meta.Title}";
        public VideoMetadata Metadata => meta;
    }
}
