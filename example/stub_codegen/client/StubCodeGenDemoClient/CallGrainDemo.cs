using System;
using System.Threading.Tasks;
using GranDen.Orleans.Client.CommonLib;
using HelloNetStandard.ShareInterface;
using Microsoft.Extensions.Logging;
using Orleans;

namespace StubCodeGenDemoClient
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
            var builder =
                NetStandard2ClientLib.ClientLib.CreateOrleansClientBuilder(clusterId: "dev", serviceId: "HelloWorldApp");

            builder.ConfigureLogging(logging => 
            {
                logging.AddProvider(CreateLoggerProvider(_logger));
            });

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

        private ILoggerProvider CreateLoggerProvider(ILogger<CallGrainDemo> logger)
        {
            return new ExistingLoggerProvider<CallGrainDemo>(logger);
        }
    }
}
