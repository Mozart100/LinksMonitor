using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    public class LinkStatistics
    {
        public long Frequency { get; set; } = 0;

        public bool IsValid { get; set; } = true;
    }

    public interface IDiscoveryGrain : IGrainWithIntegerKey
    {
        Task<LinkStatistics> GetStatisctics(string uri);
    }

}
