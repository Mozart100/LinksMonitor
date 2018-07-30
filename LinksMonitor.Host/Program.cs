using System;
using LinksMonitor.Grains.Stateless;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Storage;
using Orleans.Telemetry.SerilogConsumer;
using Serilog;

namespace LinksMonitor.Host
{
    /// <summary>
    /// Orleans test silo host
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // First, configure and start a local silo
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo();
            siloConfig.Globals.RegisterStorageProvider<MemoryStorage>("OrleansStorage");
            siloConfig.Globals.Application.SetDefaultCollectionAgeLimit(ageLimit: TimeSpan.FromMinutes(1));

            siloConfig.Globals.Application.SetCollectionAgeLimit(type: typeof(LinkStage0Grain), ageLimit: TimeSpan.FromMinutes(1));
            siloConfig.Globals.Application.SetCollectionAgeLimit(type: typeof(LinkStage1Grain), ageLimit: TimeSpan.FromMinutes(2));
            siloConfig.Globals.Application.SetCollectionAgeLimit(type: typeof(LinkStage2Grain), ageLimit: TimeSpan.FromMinutes(3));

            var logger = new Serilog.LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();
            var serilogConsumer = new SerilogConsumer(logger);

            LogManager.LogConsumers.Add(serilogConsumer);
            LogManager.TelemetryConsumers.Add(serilogConsumer);

            var silo = new SiloHost("TestSilo", siloConfig);
            silo.InitializeOrleansSilo();
            silo.StartOrleansSilo();

            Console.WriteLine("Silo started.");

            // Then configure and connect a client.
            var clientConfig = ClientConfiguration.LocalhostSilo();
            var client = new ClientBuilder().UseConfiguration(clientConfig).Build();
            client.Connect().Wait();

            Console.WriteLine("Client connected.");


            ////
            //// This is the place for your test code.
            ////

            //ClientCall(clientConfig, client);


            Console.WriteLine("\nPress Enter to terminate...");
            Console.ReadLine();

            // Shut down
            //client.Close();
            silo.ShutdownOrleansSilo();
        }

        private static void ClientCall(ClientConfiguration clientConfig, IClusterClient client)
        {
            var friend = client.GetGrain<IPageDownloaderGrain>("arkovean");
            //var friend = client.GetGrain<IGrainPageDownloader>(Guid.NewGuid()).DownloadPage("http://en.wikipedia.org/");
            //var friend = client.GetGrain<IGrain1>(0);
            //var result = friend.SayHello().Result;

        }
    }
}
