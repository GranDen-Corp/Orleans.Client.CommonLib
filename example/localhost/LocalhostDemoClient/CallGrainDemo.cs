using System.Threading.Tasks;
using GranDen.Orleans.Client.CommonLib;
using HelloWorld.ShareInterface;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;

namespace LocalhostDemoClient
{
    public class CallGrainDemo
    {
        private readonly ILogger<CallGrainDemo> _logger;

        public CallGrainDemo(ILogger<CallGrainDemo> logger)
        {
            _logger = logger;
        }

        public async Task DemoRun()
        {
            var builder = OrleansClientBuilder.CreateLocalhostClientBuilder(clusterId: "dev", serviceId: "HelloWorldApp");

            builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IHello).Assembly).WithCodeGeneration());

            //using (var client =
            //    OrleansClientBuilder.CreateLocalhostClient(_logger, clusterId: "dev", serviceId: "HelloWorldApp", applicationPartTypes: new[] { typeof(CallGrainDemo) }))
            using (var client = builder.Build())
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