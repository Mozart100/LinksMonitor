using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateful;
using Orleans.Providers;
using LinksMonitor.Interfaces.Stateless;

namespace LinksMonitor.Grains.Stateful
{
    public class LinkStage2GrainState
    {
        public string Link { get; set; } = string.Empty;

        public long AmountWasCalled { get; set; } = 0;

        public string Content { get; set; }

    }


    [StorageProvider(ProviderName = "OrleansStorage")]
    /// <summary>
    /// Grain implementation class LinkStage2Grain.
    /// </summary>
    public class LinkStage2Grain : Grain<LinkStage2GrainState>, ILinkStage2Grain
    {
        private IGrainPageDownloader _pageDownloader;

        // TODO: replace placeholder grain interface with actual grain
        // communication interface(s).

        public override Task OnActivateAsync()
        {
            _pageDownloader = GrainFactory.GetGrain<IGrainPageDownloader>(0);
            //_pageDownloader = GrainFactory.GetGrain<IGrainPageDownloader>(this.GetPrimaryKeyString().GetHashCode());


            return base.OnActivateAsync();
        }


        public async Task<LinkStatistics> GetStatistics()
        {
            if (string.IsNullOrEmpty(this.State.Content))
            {
                var response = await _pageDownloader.DownloadPage(this.GetPrimaryKeyString());
                State.Content = response.Content;
            }

            var amount = ++this.State.AmountWasCalled;
            await this.WriteStateAsync();

            return new LinkStatistics { AmountWasCalled = amount };
        }
    }
}
