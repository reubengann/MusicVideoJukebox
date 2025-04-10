using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class AvailableTrackViewModel : BaseViewModel
    {
        private readonly VideoMetadata meta;

        public AvailableTrackViewModel(VideoMetadata meta)
        {
            this.meta = meta;
        }

        public string Artist => meta.Artist;
        public string Title => meta.Title;
        public int? ReleaseYear => meta.ReleaseYear;
        public VideoMetadata Metadata => meta;

        private bool isInPlaylist;
        public bool IsInPlaylist { 
            get => isInPlaylist; 
            set => SetProperty(ref isInPlaylist, value); 
        }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set 
            { 
                SetProperty(ref isSelected, value); 
            }
        }
    }
}
