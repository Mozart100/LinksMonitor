using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Concurrency;
using System.Net;
using System.IO;
using System.Net.Http;

namespace LinksMonitor.Grains.Stateless
{
    [StatelessWorker]
    public class GrainPageDownloader : Grain, IGrainPageDownloader
    {
        public async Task<bool> DownloadPage(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.GetAsync(uri))
                {
                    using (HttpContent content = response.Content)
                    {
                        string result = await content.ReadAsStringAsync();
                    }
                }
            }

            return true;
        }
    }
}
