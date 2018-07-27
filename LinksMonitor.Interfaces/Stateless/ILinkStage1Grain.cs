using LinksMonitor.Interfaces.Stateful;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    public interface ILinkStage1Grain : ILinkStage , IGrainWithStringKey
    {
    }
}
