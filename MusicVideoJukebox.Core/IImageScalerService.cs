namespace MusicVideoJukebox.Core
{
    public interface IImageScalerService
    {
        Task<string?> ScaleImage(string libraryPath, string inputImagePath, string outputImageName);
    }
}
