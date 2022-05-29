using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Insomnia.AvatarBot.BI.Interfaces
{
    public interface ICommands
    {
      //  Task<Stream> GenerateImage(string url, int number);

        Task<Stream> GenerateImage(string url, int number);

        Task<Stream> GenerateImageToStream(IFormFile file, int number);
    }
}
