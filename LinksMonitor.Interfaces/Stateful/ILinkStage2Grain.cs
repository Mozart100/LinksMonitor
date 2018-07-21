using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateful
{
    public class LinkStatistics
    {
        public long AmountWasCalled { get; set; }
    }

    /// <summary>
    /// Grain interface ILinkStage2Grain
    /// </summary>
    public interface ILinkStage2Grain : IGrainWithStringKey
    {
        Task<LinkStatistics> GetStatistics();
    }
}
