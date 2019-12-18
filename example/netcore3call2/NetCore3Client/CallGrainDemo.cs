using System.Threading.Tasks;
using GranDen.Orleans.Client.CommonLib;
using HelloNetStandard2_1.ShareInterface;
using Microsoft.Extensions.Logging;

namespace NetCore3Client
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
            using var client = OrleansClientBuilder.CreateLocalhostClient(_logger, clusterId: "dev", serviceId: "HelloWorldApp");
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
