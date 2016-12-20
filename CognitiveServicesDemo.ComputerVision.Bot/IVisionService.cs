using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CognitiveServicesDemo.ComputerVision.Bot
{
    public interface IVisionService
    {
        Task<string> GetCaptionAsync(Stream stream);
        
        Task<string> GetCaptionAsync(string url);
    }
}