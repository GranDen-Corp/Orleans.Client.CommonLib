using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Orleans.Client.CommonLib.TypedOptions;
using Orleans.Configuration;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GranDen.Orleans.Client.CommonLib
{
    public static class OrleansClientBuilder
    {
        public static IClusterClient CreateClient(ILogger logger,
            ClusterInfoOption clusterInfo,
            OrleansProviderOption providerOption,
            IEnumerable<Type> applicationPartTypes)
        {
            var clientBuilder = new ClientBuilder();

            clientBuilder.Configure<ClientMessagingOptions>(options =>
            {
                    options.ResponseTimeout = TimeSpan.FromSeconds(20);
                    options.ResponseTimeoutWithDebugger = TimeSpan.FromMinutes(60);
            }).Configure<ClusterOptions>(options =>
            {
                    options.ClusterId = clusterInfo.ClusterId;
                    options.ServiceId = clusterInfo.ServiceId;
            });

            if (providerOption.DefaultProvider == "MongoDB")
            {
                clientBuilder.UseMongoDBClustering(options =>
                {
                    var mongoSetting = providerOption.MongoDB.Cluster;

                    options.ConnectionString = mongoSetting.DbConn;
                    options.DatabaseName = mongoSetting.DbName;

                    options.CollectionPrefix = mongoSetting.CollectionPrefix;
                });
            }

            clientBuilder.ConfigureApplicationParts(manager =>
            {
                foreach (var applicationPartType in applicationPartTypes)
                {
                    manager.AddApplicationPart(applicationPartType.Assembly).WithReferences();
                }
            }).ConfigureLogging(builder => { builder.AddSerilog(dispose: true); });

            try
            {
                return clientBuilder.Build();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create Silo Client failed");
                throw;
            }
        }
    }
}
