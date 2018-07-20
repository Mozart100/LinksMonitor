using System;
using Ark.StepRunner;
using Autofac;
using LinkMonitor.Integration.SmokTest.Scenarios;
using Serilog;
using Serilog.Core;

namespace LinkMonitor.Integration.SmokTest
{
    class Program
    {

        const string OutTemplate = "{Environment} | {Timestamp:yyyy-MM-dd HH:mm:ss} | [{Level}] | {Message}{NewLine}{Exception}";

        //const string OutTemplate = " {Environment} | {Timestamp:yyyy-MM-dd HH:mm:ss} | [{Level}] | {Bookmark} | {Message}{NewLine}{Exception}";

        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------

        private static Logger _logger;

        static void Main(string[] args)
        {
            //var uri = new Uri("http://localhost:9200");
            //var singleConnectionPool = new SingleNodeConnectionPool(uri);
            //var settings = new ConnectionConfiguration(singleConnectionPool);

            //var elasticLowLevelClient = new ElasticLowLevelClient(settings);

            CreateLogger();
            var container = InitDi();

            var stepRunner = new ScenarioRunner(_logger, container);
            var scenarioResult = stepRunner.RunScenario<CountBrokenLinksScenario>();

#if DEBUG
            _logger.Information("-------------------------------------------------------------------------------------------------------------------------------------");
            _logger.Information("-------------------------------------------------------------------------------------------------------------------------------------");
            _logger.Information("-------------------------------------------------------------------------------------------------------------------------------------");

            _logger.Information("Please press any key in order to exit.");
            Console.ReadKey();
#endif


            //elasticLowLevelClient.clos();



        }

        private static void CreateLogger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: OutTemplate)
                //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                //{

                //    AutoRegisterTemplate = true,
                //    //MinimumLogEventLevel = (LogEventLevel)esConfig.MinimumLogEventLevel,
                //    CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                //    IndexFormat = "ark-personstorage"
                //})
                .Enrich.WithProperty("Environment", "Developer")
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        private static IContainer InitDi()
        {
            var container = new ContainerBuilder();

            container.RegisterType<CountBrokenLinksScenario>();
            container.Register<ILogger>(x => _logger);
            return container.Build();
        }
    }
}
