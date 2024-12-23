
namespace MusicVideoJukebox.Core
{
    public interface IStreamAnalyzer
    {
        Task<VideoFileAnalyzeFullResult> Analyze(string path);
    }
}