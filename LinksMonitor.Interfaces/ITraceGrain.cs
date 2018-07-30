using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinksMonitor.Interfaces
{
    public interface ITraceGrain : IGrainObserver
    {
        void ReceiveMessage(string message);
    }
}
