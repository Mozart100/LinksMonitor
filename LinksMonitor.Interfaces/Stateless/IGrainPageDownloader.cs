using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    public class PageDownloaderResponse
    {
        public int  StatusCode { get; set; }
        public string Content { get; set; }
    }
    public interface IGrainPageDownloader : IGrainWithIntegerKey
    {
        Task<PageDownloaderResponse> DownloadPage(string uri);
    }

}
