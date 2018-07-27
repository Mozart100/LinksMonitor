using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateful
{
    public class LinkStatistics
    {
        public long Frequency { get; set; }

        public bool IsValid => Frequency != 0;


    }

    /// <summary>
    /// Grain interface ILinkStage2Grain
    /// </summary>
    public interface ILinkStage2Grain : IGrainWithStringKey
    {
        Task<LinkStatistics> GetStatistics();
    }
}
