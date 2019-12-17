using GranDen.Orleans.Client.CommonLib;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HelloNetCore3.ShareInterface;

namespace MongoDbConsoleClient
{
    internal class CallGrainDemo
    {
        private readonly ILogger<CallGrainDemo> _logger;

        public CallGrainDemo(ILogger<CallGrainDemo> logger)
        {
            _logger = logger;
        }

        public async Task DemoRun()
        {
            var clusterInfoOption = new ClusterInfoOption { ClusterId = "dev", ServiceId = "HelloWorldApp" };

            var mongoDbProviderOption = new OrleansProviderOption
            {
                DefaultProvider = "mongodb",
                MongoDB = new MongoDbProviderSettings 
                { 
                    Cluster = new MongoDbProviderClusterSettings 
                    { 
                        DbConn = "mongodb://localhost:27017",
                        DbName = "demo-silo-Clustering"
                    } 
                }
            };

            using var client = OrleansClientBuilder.CreateClient(_logger, clusterInfoOption, mongoDbProviderOption, new[] { typeof(IHello) });
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