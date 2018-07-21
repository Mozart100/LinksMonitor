using System;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Storage;

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
            var friend = client.GetGrain<IGrainPageDownloader>(0);
            //var friend = client.GetGrain<IGrainPageDownloader>(Guid.NewGuid()).DownloadPage("http://en.wikipedia.org/");
            //var friend = client.GetGrain<IGrain1>(0);
            //var result = friend.SayHello().Result;

        }
    }
}
