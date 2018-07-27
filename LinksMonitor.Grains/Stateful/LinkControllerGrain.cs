using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateful;
using Orleans.Providers;
using System.Collections.Generic;
using LinksMonitor.Interfaces.Stateless;

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
        private  HashSet<string> GenTwo { get; set; }

        public async Task<int> IsExisting(string uri)
        {
            //if (this.State.GenTwo.Contains(uri))
            //    return 2;

            return -1;
        }

        public async Task<LinkStatistics> Store(string uri)
        {
            
           var result =  await GrainFactory.GetGrain<ILinkStage2Grain>(uri).GetStatistics();
            return result;
        }
    }
}
