
namespace MusicVideoJukebox.Core
{
    public interface IStreamAnalyzer
    {
        Task<VideoFileAnalyzeResult> Analyze(string path);
    }
}