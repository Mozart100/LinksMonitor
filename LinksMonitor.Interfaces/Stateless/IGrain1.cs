using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    /// <summary>
    /// Grain interface IGrain1
    /// </summary>
    public interface IGrain1 : IGrainWithIntegerKey
    {
        Task<string> SayHello();
    }

}
