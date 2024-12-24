using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeImageScalerService : IImageScalerService
    {
        public List<string> ScaledImages = [];
        public string? PathToReturn;

        public async Task<string?> ScaleImage(string libraryPath, string inputImagePath, string outputImageName)
        {
            await Task.CompletedTask;
            ScaledImages.Add(inputImagePath);
            return PathToReturn;
        }
    }
}