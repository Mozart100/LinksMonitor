using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateful;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    public interface IDiscoveryGrain : IGrainWithIntegerKey
    {
        Task<LinkStatistics> GetStatisctics(string uri);
    }

}
