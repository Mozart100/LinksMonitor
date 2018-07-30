using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateful;
using Orleans.Providers;
using LinksMonitor.Interfaces.Stateless;
using System.Diagnostics;
using System;

namespace LinksMonitor.Grains.Stateless
{
    public class LinkStageGrainState
    {
        public string Link { get; set; } = string.Empty;

        public long Frequency { get; set; } = 0;
        public long TotalFrequency { get; set; } = 0;

        public string Content { get; set; }

    }


    //[StorageProvider(ProviderName = "OrleansStorage")]
    public class LinkStage2Grain : Grain, ILinkStage2Grain
    {
        private IPageDownloaderGrain _pageDownloader;
        private Stopwatch _stopwatch;
        private Random _random;
        private LinkStageGrainState _state;

        public LinkStage2Grain()
        {
            _stopwatch = new Stopwatch();
            _random = new Random();
            _state = new LinkStageGrainState();
        }

        public override Task OnActivateAsync()
        {

            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()} -  was activate!!!!!");
            _stopwatch.Start();
            _pageDownloader = GrainFactory.GetGrain<IPageDownloaderGrain>(this.GetPrimaryKeyString());
            return base.OnActivateAsync();
        }

        public async Task<LinkInfo> GetStatistics()
        {
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()} -  GetStatistics");
            var content = this._state.Content;

            if (string.IsNullOrEmpty(this._state.Content))
            {
                var response = await _pageDownloader.DownloadPage(this.GetPrimaryKeyString());
                _state.Content = content = response.Content;
            }

            var totalFrequency = ++this._state.TotalFrequency;
            var amount = ++this._state.Frequency;

            return new LinkInfo
            {
                LinkStatistics = new LinkStatistics
                {
                    Frequency = amount,
                    TotalFrequency = totalFrequency,
                    Url = this.GetPrimaryKeyString()
                },
                HtmlContent = content
            };
        }

        public async Task Init(long totalFrequency)
        {
            this._state.TotalFrequency = totalFrequency;
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
