using MusicVideoJukebox.Core;

namespace MusicVideoJukebox
{
    public class VideoInfoViewModel : BaseViewModel
    {
        private readonly VideoInfo videoInfo;

        public VideoInfoViewModel(VideoInfo videoInfo)
        {
            this.videoInfo = videoInfo;
        }

        public string Artist => videoInfo.Artist;
        public string Title => $"\"{videoInfo.Title}\"";
        public string Album => videoInfo.Album ?? "";
        public string Year => videoInfo.Year?.ToString() ?? "";
    }
}