﻿using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateless;
using Orleans.Providers;
using LinksMonitor.Interfaces.Stateful;

namespace LinksMonitor.Grains.Stateless
{
    [StorageProvider(ProviderName = "OrleansStorage")]
    public class LinkStage0Grain : Grain<LinkStage2GrainState>, ILinkStage0Grain
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
