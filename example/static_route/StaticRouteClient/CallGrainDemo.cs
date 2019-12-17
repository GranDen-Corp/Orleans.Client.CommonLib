using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GranDen.Orleans.Client.CommonLib;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using HelloNetCore3.ShareInterface;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace StaticRouteClient
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
            var clusterInfoOption = new ClusterInfoOption { ClusterId = "dev", ServiceId = "HelloWorldApp" };
            var staticGatewayOption = new StaticGatewayListProviderOptions
            {
                Gateways = new List<Uri> { new Uri("gwy.tcp://127.0.0.1:30000/0") }
            };

            using var client =
                OrleansClientBuilder.CreateStaticRouteClient(_logger, clusterInfoOption, staticGatewayOption);
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