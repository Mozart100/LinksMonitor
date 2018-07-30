using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Concurrency;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Diagnostics;

namespace LinksMonitor.Grains.Stateless
{
    //[StatelessWorker]
    public class PageDownloaderGrain : Grain, IPageDownloaderGrain
    {
        private Stopwatch _stopwatch;

        public PageDownloaderGrain()
        {
            _stopwatch = new Stopwatch();
        }

        public override Task OnActivateAsync()
        {
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()} -  was activated!!!!!");
            _stopwatch.Start();
            return base.OnActivateAsync();
        }

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

                        System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()} -  Reading Content");
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

        public override Task OnDeactivateAsync()
        {
            _stopwatch.Stop();
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()}-  was Deactivate after {_stopwatch.Elapsed.Minutes}");

            return base.OnDeactivateAsync();
        }
    }
}
