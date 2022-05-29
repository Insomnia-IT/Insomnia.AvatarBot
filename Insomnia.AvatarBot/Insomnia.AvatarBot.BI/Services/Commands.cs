using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Insomnia.AvatarBot.General.Expansions;
using Insomnia.AvatarBot.BI.Interfaces;
using System.Net;

namespace Insomnia.AvatarBot.BI.Services
{
    public class Commands : ICommands
    {
        public Commands()
        {
        }

        public async Task<Stream> GenerateImage(string url, int number)
        {
            var outputStream = new MemoryStream();
            using (var inputStream = new WebClient().OpenRead(url))
            {
                using (var image = Image.Load(inputStream))
                {
                    image.Mutate(x => x.SmartCrop(image).SmartResize(image).SmartDrawImage(image, number));
                    await image.SaveAsJpegAsync(outputStream);
                }
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }

        public async Task<Stream> GenerateImageToStream(IFormFile file, int number)
        {
            if (number <= 0 || number > 10)
                return null;

            var outputStream = new MemoryStream();
            using (var inputStream = file.OpenReadStream())
            {
                using (var image = Image.Load(inputStream))
                {
                    image.Mutate(x => x.SmartCrop(image).SmartResize(image).SmartDrawImage(image, number));
                    await image.SaveAsJpegAsync(outputStream);
                }
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }
    }
}
