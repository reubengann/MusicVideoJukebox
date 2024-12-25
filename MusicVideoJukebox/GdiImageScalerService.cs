using MusicVideoJukebox.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace MusicVideoJukebox
{
    public class GdiImageScalerService : IImageScalerService
    {
        public async Task<string?> ScaleImage(string libraryPath, string inputImagePath, string outputImageName)
        {
            var libraryImageFolder = Path.Combine(libraryPath, "images");
            if (!Directory.Exists(libraryImageFolder))
            {
                Directory.CreateDirectory(libraryImageFolder);
            }

            return await Task.Run(() =>
            {
                try
                {
                    // Load the original image
                    using var originalImage = Image.FromFile(inputImagePath);

                    int maxDimension = 256;
                    float scalingFactor = Math.Min(
                        (float)maxDimension / originalImage.Width,
                        (float)maxDimension / originalImage.Height);

                    int scaledWidth = (int)(originalImage.Width * scalingFactor);
                    int scaledHeight = (int)(originalImage.Height * scalingFactor);

                    // Create a new blank canvas
                    using var finalImage = new Bitmap(maxDimension, maxDimension, PixelFormat.Format32bppArgb);
                    using (var graphics = Graphics.FromImage(finalImage))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        graphics.Clear(Color.Transparent);

                        // Center the scaled image
                        int offsetX = (maxDimension - scaledWidth) / 2;
                        int offsetY = (maxDimension - scaledHeight) / 2;
                        graphics.DrawImage(originalImage, offsetX, offsetY, scaledWidth, scaledHeight);
                    }

                    // Save the result
                    var outputImagePath = Path.Combine(libraryImageFolder, outputImageName);
                    finalImage.Save(outputImagePath, ImageFormat.Png);

                    return Path.Combine("images", outputImageName);
                }
                catch (Exception ex)
                {
                    return null;
                }
            });
        }

    }
}