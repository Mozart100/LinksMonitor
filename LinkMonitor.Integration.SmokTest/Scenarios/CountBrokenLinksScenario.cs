using Ark.StepRunner.CustomAttribute;
using LinksMonitor.Interfaces.Stateful;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Runtime.Configuration;
using Serilog;
using Shouldly;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
//using Orleans;
//using Orleans.Runtime.Configuration;
//using BrokenLinksMonitor.Grain.Interfaces;

namespace LinkMonitor.Integration.SmokTest.Scenarios
{

    [AScenario("Will Send request of Urls")]
    class CountBrokenLinksScenario
    {
        private ILogger _logger;
        private Process _server;
        private ClientConfiguration _clientConfig;
        private IClusterClient _client;

        //private ClientConfiguration _clientConfig;
        //private IClusterClient _client;

        private enum ScenarioSteps
        {
            CleanAllRunningProcess,
            ActivateServer,
            ActivateClient,
            SendValidSingleRequest,

            DisposeEverything,
        }



        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------

        public CountBrokenLinksScenario(ILogger logger)
        {
            _logger = logger;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------

        [AStepSetupScenario((int)ScenarioSteps.CleanAllRunningProcess, "Launch Server.")]
        public void CleanAllRunningProcess()
        {
            CleanAllRunningProcesses_BrokenLinksMonitor();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        [AStepSetupScenario((int)ScenarioSteps.ActivateServer, "Launch Server.")]
        public void ActivateServer()
        {
            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(typeof(CountBrokenLinksScenario).Assembly.Location))
                .Parent
                .Parent
                .Parent;
            var brokenLinksMonitor = Path.Combine(directoryInfo.FullName, "LinksMonitor.Host", "bin", "Debug", "LinksMonitor.Host.exe");
            _server = Process.Start(brokenLinksMonitor);

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        [AStepSetupScenario((int)ScenarioSteps.ActivateClient, "Launch Server.")]
        public void ActivateClient()
        {
            _clientConfig = ClientConfiguration.LocalhostSilo();
            _client = new ClientBuilder().UseConfiguration(_clientConfig).Build();
            _client.Connect().Wait();

            _logger.Information("Client connected.");

            
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        [ABusinessStepScenario((int)ScenarioSteps.SendValidSingleRequest, "Sending single request.")]
        public void SendValidSingleRequest()
        {
            //_client.GetGrain<ILinkStage2Grain>("http://en.wikipedia.org/").GetStatistics().Result.Frequency.ShouldBe(1);
            //_client.GetGrain<ILinkStage2Grain>("http://en.wikipedia.org/").GetStatistics().Result.Frequency.ShouldBe(2);
            //_client.GetGrain<ILinkStage2Grain>("http://en.wikipedia.org/").GetStatistics().Result.Frequency.ShouldBe(3);

            var response = _client.GetGrain<IDiscoveryGrain>(0).GetStatisctics("http://en.wikipedia.org/").Result;
            response.IsValid.ShouldBeTrue();
            response.Frequency.ShouldBe(1);

            _client.GetGrain<IDiscoveryGrain>(0).GetStatisctics("http://en.wikipedia.org/").Result.Frequency.ShouldBe(2);
            _client.GetGrain<IDiscoveryGrain>(0).GetStatisctics("http://en.wikipedia.org/").Result.Frequency.ShouldBe(3);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        [AStepCleanupScenario((int)ScenarioSteps.DisposeEverything, "Disposing from all processes")]
        public void DisposeEverything()
        {
            CleanAllRunningProcesses_BrokenLinksMonitor();
            _client.Close();
        }


        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------

        private void CleanAllRunningProcesses_BrokenLinksMonitor()
        {
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName.ToLower().Contains("LinksMonitor.Host".ToLower()))
                {
                    try
                    {
                        _logger.Information("Process name = {ProcessName} is killed ", item.ProcessName);
                        item.Kill();
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, "During killing  of step CleanAllRunningProcess.");

                    }
                }
            }
        }


    }
}
