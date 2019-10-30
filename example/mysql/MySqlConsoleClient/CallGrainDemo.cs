using GranDen.Orleans.Client.CommonLib;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using HelloWorld.ShareInterface;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MySqlConsoleClient
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

            var sqlDbProviderOption =
                new OrleansProviderOption
                {
                    DefaultProvider = @"MYSQL",
                    SQLDB = new AdoNetProviderSettings
                    {
                        Cluster = new AdoNetProviderClusterSettings
                        {
                            DbConn =
                                @"Server=localhost;uid=root;pwd=Pass1234;Database=orleans_demo"
                        }
                    }
                };

            using (var client =
                OrleansClientBuilder.CreateClient(_logger, clusterInfoOption, sqlDbProviderOption, new[] { typeof(IHello) }))
            {
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
}