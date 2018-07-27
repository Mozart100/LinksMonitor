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
    public class PageDownloaderGrain : Grain, IPageDownloaderGrain
    {
        public async Task<PageDownloaderResponse> DownloadPage(string uri)
        {
            var siteContent = string.Empty;
            HttpResponseMessage response = null;
            using (HttpClient client = new HttpClient())
            {
                using (response = await client.GetAsync(uri))
                {
                    using (HttpContent content = response.Content)
                    {
                        siteContent = await content.ReadAsStringAsync();
                    }
                }
            }

            return new PageDownloaderResponse
            {
                StatusCode = 200, //response.StatusCode.,
                Content = siteContent,
            };
        }
    }
}
