using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateless;
using Orleans.Providers;
using LinksMonitor.Interfaces.Stateful;
using Serilog;
using LinksMonitor.Interfaces;
using System.Diagnostics;
using System;
using Orleans.Concurrency;

namespace LinksMonitor.Grains.Stateless
{
    //[StorageProvider(ProviderName = "OrleansStorage")]
    //[StatelessWorker]
    public class LinkStage0Grain : Grain, ILinkStage0Grain
    {
        private IPageDownloaderGrain _pageDownloader;
        private Stopwatch _stopwatch;
        private LinkStageGrainState _state;

        //private ObserverSubscriptionManager<ITraceGrain> _subsManager;

        public LinkStage0Grain()
        {
            _stopwatch = new Stopwatch();
            _state = new LinkStageGrainState();
        }

        public override Task OnActivateAsync()
        {
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()} -  was activated!!!!!");
            _stopwatch.Start();
            _pageDownloader = GrainFactory.GetGrain<IPageDownloaderGrain>(this.GetPrimaryKeyString());
            //_subsManager = new ObserverSubscriptionManager<ITraceGrain>();
            return base.OnActivateAsync();
        }


        public async Task<LinkInfo> GetStatistics()
        {
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()} -  GetStatistics");

            var copntent = _state.Content;

            if (string.IsNullOrEmpty(_state.Content))
            {
                

                var response = await _pageDownloader.DownloadPage(this.GetPrimaryKeyString());
                _state.Content = copntent = response.Content;
            }

            var amount = _state.TotalFrequency = ++_state.Frequency;

            return new LinkInfo
            {
                LinkStatistics = new LinkStatistics
                {
                    Frequency = amount,
                    TotalFrequency = amount,
                    Url = this.GetPrimaryKeyString()
                },
                HtmlContent = copntent
            };
        }

        public async Task Init(long totalFrequency)
        {
            await Task.CompletedTask;
        }

        public override Task OnDeactivateAsync()
        {
            _stopwatch.Stop();
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()}-  was Deactivate after {_stopwatch.Elapsed.Minutes}");

            return base.OnDeactivateAsync();
        }
    }
}
