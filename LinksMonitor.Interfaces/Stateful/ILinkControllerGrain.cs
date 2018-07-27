using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateful
{
    public interface ILinkControllerGrain : IGrainWithIntegerKey
    {
        Task<int> IsExisting(string uri);
        Task<LinkStatistics> Store(string uri);



    }


}
