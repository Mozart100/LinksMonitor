using Ark.StepRunner.CustomAttribute;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Runtime.Configuration;
using Serilog;
using Shouldly;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;


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
            SendInvalidSingleRequest,
            Dir,
            Watch,

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
            _clientConfig.StatisticsPerfCountersWriteInterval = TimeSpan.FromMinutes(1); 
            _clientConfig.StatisticsMetricsTableWriteInterval = TimeSpan.FromMinutes(1); 
            _clientConfig.StatisticsLogWriteInterval = TimeSpan.FromMinutes(1);
            _client = new ClientBuilder().UseConfiguration(_clientConfig).Build();
            _client.Connect().Wait();

            _logger.Information("Client connected.");
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        [ABusinessStepScenario((int)ScenarioSteps.SendValidSingleRequest, "Sending requests.")]
        public void SendValidSingleRequest()
        {
            var response = default(LinkStatistics);
            for (int loop = 0; loop < 3; loop++)
            {
                for (int expected = 1; expected <= 20; expected++)
                {
                    response = _client.GetGrain<IDiscoveryGrain>(0).GetStatisctics("http://en.wikipedia.org/").Result;
                    response.IsValid.ShouldBeTrue();
                    response.Frequency.ShouldBe(expected);
                    response.TotalFrequency.ShouldBe(loop * 20 + expected);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        [ABusinessStepScenario((int)ScenarioSteps.SendInvalidSingleRequest, "Sending invalid request.")]
        public void SendInvalidSingleRequest()
        {
            var response = _client.GetGrain<IDiscoveryGrain>(0).GetStatisctics("/w/load.php?debug=false&amp;lang=en&amp;modules=ext.gadget.charinsert-styles&amp;only=styles&amp;skin=vector").Result;
            response.IsValid.ShouldBeFalse();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        [ABusinessStepScenario((int)ScenarioSteps.Dir, "Dir https://dotnet.github.io/orleans/Documentation/Introduction.html")]
        public void Dir()
        {
            var response = _client.GetGrain<IDiscoveryGrain>(0).Dir("http://en.wikipedia.org/").Result;
            //var response = _client.GetGrain<IDiscoveryGrain>(0).Dir("https://dotnet.github.io/orleans/Documentation/Introduction.html").Result;
            response.Count.ShouldBeGreaterThan(1);

            foreach (var item in response)
            {
                _logger.Information(item);
            }
        }
        [ABusinessStepScenario((int)ScenarioSteps.Watch, "Watch until all Grains Finish")]
        public void Watch()
        {
            Thread.Sleep(TimeSpan.FromMinutes(5));
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
