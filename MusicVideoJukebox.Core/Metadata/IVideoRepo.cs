
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IVideoRepo
    {
        Task CreateTables();
        Task AddBasicInfos(List<BasicInfo> basicInfos);
    }

    public class BasicInfo
    {
        required public string Title { get; set; }
        required public string Artist { get; set; }
        required public string Filename { get; set; }
    }
}
