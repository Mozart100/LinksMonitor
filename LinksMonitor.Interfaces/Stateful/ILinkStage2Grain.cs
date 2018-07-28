using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateless;
using Orleans;

namespace LinksMonitor.Interfaces.Stateful
{
    public class LinkInfo
    {
        public LinkStatistics LinkStatistics { get; set; }
        public string HtmlContent { get; set; }

    }

    public interface ILinkStage
    {
        Task<LinkInfo> GetStatistics();

        Task Init(long totalFrequency);

    }



    public interface ILinkStage2Grain : ILinkStage, IGrainWithStringKey
    {
    }
}
