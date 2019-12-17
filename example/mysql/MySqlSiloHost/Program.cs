using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;
using HelloNetCore3.Grains;

namespace MySqlSiloHost
{
    class Program
    {
        private static readonly string dbConnStr = @"Server=localhost;uid=root;pwd=Pass1234;Database=orleans_demo";

        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .UseOrleans(siloHostBuilder =>
                {
                    siloHostBuilder
                        .UseAdoNetClustering(options =>
                        {
                            options.Invariant = "MySql.Data.MySqlClient";
                            options.ConnectionString = dbConnStr;
                        })
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
