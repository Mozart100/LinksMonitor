using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    public interface IGrainPageDownloader : IGrainWithGuidKey
    {
        Task<bool> DownloadPage(string uri);
    }

}
