namespace MusicVideoJukebox.Core.Audio
{
    public interface IStreamAnalyzer
    {
        Task<VideoFileAnalyzeFullResult> Analyze(string path, CancellationToken cancellationToken);
    }
}