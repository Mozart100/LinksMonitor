using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces
{
    /// <summary>
    /// Grain interface IGrain1
    /// </summary>
    public interface IGrain1 : IGrainWithGuidKey
    {
        Task<string> SayHello();
    }
}
