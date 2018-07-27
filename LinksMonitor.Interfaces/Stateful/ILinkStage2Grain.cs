using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateless;
using Orleans;

namespace LinksMonitor.Interfaces.Stateful
{
    public interface ILinkStage
    {
        Task<LinkStatistics> GetStatistics();
    }

    public interface ILinkStage2Grain : ILinkStage, IGrainWithStringKey
    {
    }
}
