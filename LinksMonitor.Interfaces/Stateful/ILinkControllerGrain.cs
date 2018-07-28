using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateless;
using Orleans;

namespace LinksMonitor.Interfaces.Stateful
{
    public interface ILinkControllerGrain : IGrainWithIntegerKey
    {
        Task<LinkInfo> Store(string uri,bool withSubUrls);
    }
}
