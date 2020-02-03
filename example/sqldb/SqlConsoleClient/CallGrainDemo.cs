using GranDen.Orleans.Client.CommonLib;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HelloNetCore3.ShareInterface;
using Serilog;

namespace SqlConsoleClient
{
    class CallGrainDemo
    {
        private readonly ILogger<CallGrainDemo> _logger;

        public CallGrainDemo(ILogger<CallGrainDemo> logger)
        {
            _logger = logger;
        }

        public async Task DemoRun()
        {
            var clusterInfoOption = new ClusterInfoOption { ClusterId = "dev", ServiceId = "HelloWorldApp" };

            var sqlDbProviderOption =
                new OrleansProviderOption
                {
                    DefaultProvider = @"SQLDB",
                    SQLDB = new AdoNetProviderSettings
                    {
                        Cluster = new AdoNetProviderClusterSettings
                        {
                            DbConn =
                                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Orleans_Cluster;
                                Integrated Security=True;Pooling=False;Max Pool Size=200;
                                MultipleActiveResultSets=True"
                        }
                    }
                };

            using var client =
                OrleansClientBuilder.CreateClient(_logger,
                clusterInfoOption, sqlDbProviderOption,
                configureLogging: builder => { builder.AddSerilog(); });

            await client.ConnectWithRetryAsync();
            _logger.LogInformation("Client successfully connect to silo host");

            var grain = client.GetGrain<IHello>(0);
            _logger.LogInformation("Get hello world grain, start calling RPC methods...");

            var returnValue = await grain.SayHello("Hello Orleans");
            _logger.LogInformation($"RPC method return value is \r\n\r\n{{{returnValue}}}\r\n");

            await client.Close();
            _logger.LogInformation("Client successfully close connection to silo host");
        }
    }
}
