using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateful;
using Orleans.Providers;
using System.Collections.Generic;
using LinksMonitor.Interfaces.Stateless;
using System.Collections.Concurrent;

namespace LinksMonitor.Grains.Stateful
{
    public class LinkControllerState
    {
        public HashSet<string> GenTwo { get; set; }

    }

    //[StorageProvider(ProviderName = "OrleansStorage")]
    /// <summary>
    /// Grain implementation class LinkStage2Grain.
    /// </summary>
    public class LinkControllerGrain : Grain, ILinkControllerGrain
    {
        private ConcurrentDictionary<string, int> _storage;
        private IValidationUrlGrain _validationUrl;

        public LinkControllerGrain()
        {
            _storage = new ConcurrentDictionary<string, int>();
        }

        public override Task OnActivateAsync()
        {
            _validationUrl = GrainFactory.GetGrain<IValidationUrlGrain>(0);
            return base.OnActivateAsync();
        }

        public async Task<int> IsExisting(string uri)
        {
            //if (this.State.GenTwo.Contains(uri))
            //    return 2;

            return -1;
        }

        public async Task<LinkStatistics> Store(string uri, bool withSubUrls)
        {
            var result = await StoreUri(uri);

            if (withSubUrls)
            {
                foreach (var item in await _validationUrl.ExtractValidUrls(result.HtmlContent))
                {
                    await StoreUri(item);
                }
            }

            return result.LinkStatistics;
        }

        private async Task<LinkInfo> StoreUri(string uri)
        {
            var stage = _storage.GetOrAdd(uri, key => 0);

            var result = default(LinkInfo);
            switch (stage)
            {

                case 0:
                    result = await GrainFactory.GetGrain<ILinkStage0Grain>(uri).GetStatistics();
                    if (result.LinkStatistics.Frequency > 20)
                    {
                        result = await GrainFactory.GetGrain<ILinkStage1Grain>(uri).GetStatistics();
                        _storage.AddOrUpdate(uri, 1, (key, value) => value + 1);
                    }
                    break;

                case 1:
                    result = await GrainFactory.GetGrain<ILinkStage1Grain>(uri).GetStatistics();
                    if (result.LinkStatistics.Frequency > 20)
                    {
                        result = await GrainFactory.GetGrain<ILinkStage2Grain>(uri).GetStatistics();
                        _storage.AddOrUpdate(uri, 2, (key, value) => value + 1);
                    }
                    break;

                case 2:
                    result = await GrainFactory.GetGrain<ILinkStage2Grain>(uri).GetStatistics();
                    break;

                default:
                    break;
            }

            return result;
        }
    }
}
