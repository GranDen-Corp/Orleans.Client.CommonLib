using System;
using System.Net;
using System.Threading.Tasks;
using HelloWorld.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace SqlSiloHost
{
    class Program
    {
        private static readonly string dbConnStr =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Orleans;
            Integrated Security=True;Pooling=False;Max Pool Size=200;
            MultipleActiveResultSets=True";

        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .UseOrleans(siloHostBuilder =>
                {
                    siloHostBuilder
                        .UseAdoNetClustering(options =>
                        {
                            options.Invariant = "System.Data.SqlClient";
                            options.ConnectionString = dbConnStr;
                        })
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
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
