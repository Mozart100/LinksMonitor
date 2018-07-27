using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateful;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Concurrency;

namespace LinksMonitor.Grains.Stateless
{
    [StatelessWorker]
    public class DiscoveryGrain : Grain, IDiscoveryGrain
    {
        private ILinkControllerGrain _linkController;
        private IValidationUrlGrain _validationUrl;

        public override Task OnActivateAsync()
        {
            _linkController = GrainFactory.GetGrain<ILinkControllerGrain>(0);
            _validationUrl = GrainFactory.GetGrain<IValidationUrlGrain>(0);
            return base.OnActivateAsync();
        }

        public async Task<LinkStatistics> GetStatisctics(string uri)
        {
            var isvalid = await _validationUrl.Validate(uri);
            if (isvalid == false)
            {
                return new LinkStatistics { IsValid = false };
            }
            return await _linkController.Store(uri);
        }
    }
}
