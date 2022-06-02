using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Insomnia.AvatarBot.General.Expansions
{
    public static class ImageExpansions
    {
        public static IImageProcessingContext SmartResize(this IImageProcessingContext context, Image image)
        {
            var absoluteSize = GetAbsoluteSize(image);

            var newSize = NewSize(absoluteSize);

            return context.Resize(newSize, newSize);
        }

        // 400 720 853 1280
        private static int NewSize(int absoluteSize)
        {
            //Идея такая: если размер изображения ближе по размеру к, например 400 чем к 720, берётся 400. А если ближе к 720, уже берётся 720.
            if (absoluteSize < (400 + ((720 - 400) / 2)))
                return 400;
            if (absoluteSize < (720 + ((853 - 720) / 2)))
                return 720;
            if (absoluteSize < (853 + ((1280 - 853) / 2)))
                return 853;

            return 1280;
        }

        private static int GetAbsoluteSize(Image image) => image.Width < image.Height? image.Width : image.Height;

        public static IImageProcessingContext SmartCrop(this IImageProcessingContext context, Image image)
        {
            var absoluteSize = GetAbsoluteSize(image);

            var x = image.Width / 2 - absoluteSize / 2;
            var y = image.Height / 2 - absoluteSize / 2;

            return context.Crop(new Rectangle(x, y, absoluteSize, absoluteSize));
        }

        public static IImageProcessingContext SmartDrawImage(this IImageProcessingContext context, Image image, int number)
        {
            using (Image frame = Image.Load(GetFramePath(GetAbsoluteSize(image), number)))
            {
                return context.DrawImage(frame, 1);
            }
        }

        public static string GetMainImagePath() => GetPath(null, GetMainImage());

        private static string GetFramePath(int absoluteSize, int number) =>
            GetPath(GetDirection(absoluteSize), GetFileName(number));

        private static string GetPath(string description, string file) =>
            @$"Insomnia_frames_21-05{(String.IsNullOrEmpty(description) ? "" : $"/{description}")}/{file}";

        private static string GetDirection(int absoluteSize) =>
            absoluteSize switch
            {
                400 => "400-400",
                720 => "720-720",
                853 => "853-853",
                1280 => "1280-1280",
                _ => "400-400"
            };

        private static string GetFileName(int number) => $"{number}.png";

        private static string GetMainImage() => "insomnia_post_vk.jpg";
    }
}
