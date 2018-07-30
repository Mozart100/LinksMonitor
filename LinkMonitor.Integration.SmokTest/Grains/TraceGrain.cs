using LinksMonitor.Interfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkMonitor.Integration.SmokTest.Grains
{
    public class TraceGrain : ITraceGrain
    {
        public void ReceiveMessage(string message)
        {
            
        }
    }
}
