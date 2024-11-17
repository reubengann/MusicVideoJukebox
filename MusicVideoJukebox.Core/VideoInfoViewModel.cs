namespace MusicVideoJukebox.Core
{
    public class VideoInfoViewModel(VideoInfo videoInfo) : BaseViewModel
    {
        private readonly VideoInfo videoInfo = videoInfo;

        public string Artist => videoInfo.Artist;
        public string Title => $"\"{videoInfo.Title}\"";
        public string Album => videoInfo.Album ?? "";
        public string Year => videoInfo.Year?.ToString() ?? "";
    }
}