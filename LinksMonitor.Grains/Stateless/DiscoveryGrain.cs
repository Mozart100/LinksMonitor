using System.Collections.Generic;
using System.Linq;
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

            var response = await _linkController.Store(uri, withSubUrls: false);
            return response.LinkStatistics;
        }

        public async Task<IList<string>> Dir(string uri)
        {
            var validUrls = new List<string>();

            try
            {
                var response = await _linkController.Store(uri, withSubUrls: false);
                var tasks = new List<Task<LinkInfo>>();
                var loop = 0;
                foreach (var item in await _validationUrl.ExtractValidUrls(htmlContent: response.HtmlContent))
                {
                    loop++;

                    if (loop == 10)
                    {
                        await Task.WhenAll(tasks.ToArray());
                        foreach (var stat in tasks.Select(x => x.Result).Where(x => x.LinkStatistics.IsValid))
                        {
                            validUrls.Add(stat.LinkStatistics.Url);
                        }
                        tasks.Clear();
                    }
                    tasks.Add(_linkController.Store(item, withSubUrls: false));
                }

                await Task.WhenAll(tasks.ToArray());
                foreach (var stat in tasks.Select(x => x.Result).Where(x => x.LinkStatistics.IsValid))
                {
                    validUrls.Add(stat.LinkStatistics.Url);
                }

            }
            catch (System.Exception exception)
            {

                int x = 0;
            }


            return validUrls;
        }
    }
}
