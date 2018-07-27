using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    public class LinkStatistics
    {
        public long Frequency { get; set; }

        public bool IsValid => Frequency != 0;
    }

    public interface IDiscoveryGrain : IGrainWithIntegerKey
    {
        Task<LinkStatistics> GetStatisctics(string uri);
    }

}
