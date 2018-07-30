﻿using System.Threading.Tasks;
using Orleans;
using LinksMonitor.Interfaces.Stateless;
using Orleans.Providers;
using LinksMonitor.Interfaces.Stateful;
using System.Diagnostics;
using System;

namespace LinksMonitor.Grains.Stateless
{
    //[StorageProvider(ProviderName = "OrleansStorage")]
    public class LinkStage1Grain : Grain, ILinkStage1Grain
    {
        private IPageDownloaderGrain _pageDownloader;
        private Stopwatch _stopwatch;
        private Random _random;
        private LinkStageGrainState _state;

        public LinkStage1Grain()
        {
            _stopwatch = new Stopwatch();
            _random = new Random();

            _state = new LinkStageGrainState();
        }

        public override Task OnActivateAsync()
        {
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()}-  was activate!!!!!");
            _stopwatch.Start();
            _pageDownloader = GrainFactory.GetGrain<IPageDownloaderGrain>(this.GetPrimaryKeyString());
            return base.OnActivateAsync();
        }


        public async Task<LinkInfo> GetStatistics()
        {
            System.Console.WriteLine($"{this.GetType().Name} {this.GetPrimaryKeyString()} -  GetStatistics");

            var copntent = _state.Content;

            if (string.IsNullOrEmpty(this._state.Content))
            {
                var response = await _pageDownloader.DownloadPage(this.GetPrimaryKeyString());
                _state.Content = copntent = response.Content;
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
                HtmlContent = copntent
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
