using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateful;
using Orleans.Providers;
using System.Collections.Generic;
using LinksMonitor.Interfaces.Stateless;
using System.Collections.Concurrent;
using Orleans.Concurrency;

namespace LinksMonitor.Grains.Stateful
{
    public class LinkControllerState
    {
        public HashSet<string> GenTwo { get; set; }

    }

    //[StorageProvider(ProviderName = "OrleansStorage")]
    [Reentrant]
    public class LinkControllerGrain : Grain, ILinkControllerGrain
    {
        const int LevelUp = 20;

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

        public async Task<LinkInfo> Store(string uri)
        {
            var result = await StoreUri(uri);

            return result;
        }

        private async Task<LinkInfo> StoreUri(string uri)
        {
            var stage = _storage.GetOrAdd(uri, key => 0);

            var result = default(LinkInfo);
            switch (stage)
            {
                case 0:
                    result = await GrainFactory.GetGrain<ILinkStage0Grain>(uri).GetStatistics();
                    if (result.LinkStatistics.Frequency > LevelUp)
                    {
                        await GrainFactory.GetGrain<ILinkStage1Grain>(uri).Init(LevelUp);
                        result = await GrainFactory.GetGrain<ILinkStage1Grain>(uri).GetStatistics();
                        _storage.AddOrUpdate(uri, 1, (key, value) => value + 1);
                    }
                    break;

                case 1:
                    result = await GrainFactory.GetGrain<ILinkStage1Grain>(uri).GetStatistics();
                    if (result.LinkStatistics.Frequency > LevelUp)
                    {
                        await GrainFactory.GetGrain<ILinkStage2Grain>(uri).Init(LevelUp * 2);
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
