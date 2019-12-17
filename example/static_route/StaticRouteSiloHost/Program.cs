using System;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Threading.Tasks;
using HelloNetCore3.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;

namespace StaticRouteSiloHost
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseDevelopmentClustering(new IPEndPoint(IPAddress.Loopback, 11111))
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
                        })
                        .Configure<StatisticsOptions>(options =>
                        {
                            options.CollectionLevel = StatisticsLevel.Critical;
                        })
                        .Configure<EndpointOptions>(options =>
                        {
                            options.AdvertisedIPAddress = IPAddress.Loopback;
                            options.GatewayPort = 30000;
                            options.SiloPort = 11111;
                        })
                        .ConfigureApplicationParts(parts =>
                        {
                            parts.AddFromDependencyContext(typeof(HelloGrain).Assembly);
                        });
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder => { builder.AddConsole(); })
                .UseConsoleLifetime();

            try
            {
                await hostBuilder.RunConsoleAsync();
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Temporary failure, ex={ex}");
            }
        }
    }
}
