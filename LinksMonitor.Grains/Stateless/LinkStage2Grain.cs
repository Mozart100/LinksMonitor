using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateful;
using Orleans.Providers;
using LinksMonitor.Interfaces.Stateless;

namespace LinksMonitor.Grains.Stateless
{
    public class LinkStageGrainState
    {
        public string Link { get; set; } = string.Empty;

        public long Frequency { get; set; } = 0;
        public long TotalFrequency { get; set; } = 0;

        public string Content { get; set; }

    }


    [StorageProvider(ProviderName = "OrleansStorage")]
    public class LinkStage2Grain : Grain<LinkStageGrainState>, ILinkStage2Grain
    {
        private IPageDownloaderGrain _pageDownloader;

        public override Task OnActivateAsync()
        {
            _pageDownloader = GrainFactory.GetGrain<IPageDownloaderGrain>(0);
            return base.OnActivateAsync();
        }

        public async Task<LinkInfo> GetStatistics()
        {
            var content = this.State.Content;

            if (string.IsNullOrEmpty(this.State.Content))
            {
                var response = await _pageDownloader.DownloadPage(this.GetPrimaryKeyString());
                State.Content = content = response.Content;
            }

            var totalFrequency = ++this.State.TotalFrequency;
            var amount = ++this.State.Frequency;
            await this.WriteStateAsync();

            return new LinkInfo
            {
                LinkStatistics = new LinkStatistics
                {
                    Frequency = amount,
                    TotalFrequency = totalFrequency,
                    Url = this.GetPrimaryKeyString()
                },
                HtmlContent = content
            };
        }

        public async Task Init(long totalFrequency)
        {
            await this.ReadStateAsync();
            this.State.TotalFrequency = totalFrequency;
            await this.WriteStateAsync();

            await Task.CompletedTask;
        }
    }
}
