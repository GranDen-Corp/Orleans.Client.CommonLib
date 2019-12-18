using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NetCore2SiloHost
{
    class Program
    {
        static async Task Main(string[] _)
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
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);
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
