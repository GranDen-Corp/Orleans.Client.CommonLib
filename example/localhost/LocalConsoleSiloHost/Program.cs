using Microsoft.Extensions.Hosting;
using System.Net;
using System.Threading.Tasks;
using HelloWorld.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Orleans.Runtime.Configuration;

namespace LocalConsoleSiloHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
                        })
                        .Configure<StatisticsOptions>(options =>
                        {
                            options.CollectionLevel = StatisticsLevel.Critical;
                        })
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
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
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                });

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
