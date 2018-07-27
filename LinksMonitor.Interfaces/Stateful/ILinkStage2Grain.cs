using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateless;
using Orleans;

namespace LinksMonitor.Interfaces.Stateful
{
    

    /// <summary>
    /// Grain interface ILinkStage2Grain
    /// </summary>
    public interface ILinkStage2Grain : IGrainWithStringKey
    {
        Task<LinkStatistics> GetStatistics();
    }
}
