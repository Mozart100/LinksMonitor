using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateful;
using Orleans.Providers;
using LinksMonitor.Interfaces.Stateless;

namespace LinksMonitor.Grains.Stateless
{
    public class LinkStage2GrainState
    {
        public string Link { get; set; } = string.Empty;

        public long Frequency { get; set; } = 0;

        public string Content { get; set; }

    }


    [StorageProvider(ProviderName = "OrleansStorage")]
    /// <summary>
    /// Grain implementation class LinkStage2Grain.
    /// </summary>
    public class LinkStage2Grain : Grain<LinkStage2GrainState>, ILinkStage2Grain
    {
        private IPageDownloaderGrain _pageDownloader;

        public override Task OnActivateAsync()
        {
            _pageDownloader = GrainFactory.GetGrain<IPageDownloaderGrain>(0);
            return base.OnActivateAsync();
        }


        public async Task<LinkInfo> GetStatistics()
        {
            var copntent = "";

            if (string.IsNullOrEmpty(this.State.Content))
            {
                var response = await _pageDownloader.DownloadPage(this.GetPrimaryKeyString());
                State.Content = copntent = response.Content;
            }

            var amount = ++this.State.Frequency;
            await this.WriteStateAsync();

            return new LinkInfo { LinkStatistics = new LinkStatistics { Frequency = amount, Url = this.GetPrimaryKeyString() }, HtmlContent = copntent };
        }
    }
}
