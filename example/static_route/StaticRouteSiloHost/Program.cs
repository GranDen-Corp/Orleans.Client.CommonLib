﻿using Microsoft.Extensions.Hosting;
using System.Net;
using System.Threading.Tasks;
using HelloWorld.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace StaticRouteSiloHost
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseDevelopmentClustering(new IPEndPoint(IPAddress.Loopback, 11111))
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
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
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .UseConsoleLifetime()
                .RunConsoleAsync();
        }
    }
}