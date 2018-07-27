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

        public LinkControllerGrain()
        {
            _storage = new ConcurrentDictionary<string, int>();
        }

        public async Task<int> IsExisting(string uri)
        {
            //if (this.State.GenTwo.Contains(uri))
            //    return 2;

            return -1;
        }

        public async Task<LinkStatistics> Store(string uri)
        {
            //if () 
            var result = await GrainFactory.GetGrain<ILinkStage2Grain>(uri).GetStatistics();
            return result;
        }



    }
}
