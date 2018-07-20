using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Grains
{
    /// <summary>
    /// Grain implementation class Grain1.
    /// </summary>
    public class Grain1 : Grain, LinksMonitor.Interfaces.IGrain1
    {
        public Task<string> SayHello()
        {
            return Task.FromResult("Hello World!");
        }
    }
}
