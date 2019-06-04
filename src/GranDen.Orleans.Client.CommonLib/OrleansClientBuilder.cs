﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using Orleans.Configuration;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Orleans;

namespace GranDen.Orleans.Client.CommonLib
{
    /// <summary>
    /// Helper Class for easier creating Orleans RPC Client
    /// </summary>
    public static class OrleansClientBuilder
    {
        /// <summary>
        /// Create Orleans Client using various configuration options
        /// </summary>
        /// <param name="logger">Logger to log ClientBuilder operation information</param>
        /// <param name="clusterInfo"></param>
        /// <param name="providerOption"></param>
        /// <param name="applicationPartTypes">Application parts (optional)</param>
        /// <param name="usingSerilog">Default use Serilog (https://serilog.net), set to <c>false</c> to use Orleans' original logging.</param>
        /// <returns></returns>
        public static IClusterClient CreateClient(ILogger logger,
            ClusterInfoOption clusterInfo,
            OrleansProviderOption providerOption,
            IEnumerable<Type> applicationPartTypes = null,
            bool usingSerilog = true)
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
                logger.LogTrace("Using MongoDB provider...");
                clientBuilder.UseMongoDBClustering(options =>
                {
                    var mongoSetting = providerOption.MongoDB.Cluster;

                    options.ConnectionString = mongoSetting.DbConn;
                    options.DatabaseName = mongoSetting.DbName;

                    options.CollectionPrefix = mongoSetting.CollectionPrefix;
                });
            }

            if (applicationPartTypes != null)
            {
                clientBuilder.ConfigureApplicationParts(manager =>
                {
                    foreach (var applicationPartType in applicationPartTypes)
                    {
                        manager.AddApplicationPart(applicationPartType.Assembly).WithReferences();
                    }
                });
            }

            if (usingSerilog)
            {
                clientBuilder.ConfigureLogging(builder => { builder.AddSerilog(dispose: true); });
            }

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

        /// <summary>
        /// Create a local connect only silo host client
        /// </summary>
        /// <param name="logger">Logger to log ClientBuilder operation information</param>
        /// <param name="gatewayPort"></param>
        /// <param name="serviceId"></param>
        /// <param name="clusterId"></param>
        /// <param name="usingSerilog">Default use Serilog (https://serilog.net), set to <c>false</c> to use Orleans' original logging.</param>
        /// <returns></returns>
        public static IClusterClient CreateLocalhostClient(ILogger logger,
            int gatewayPort = 30000,
            string clusterId = "dev",
            string serviceId = "dev",
            bool usingSerilog = true)
        {
            var clientBuilder = new ClientBuilder().UseLocalhostClustering(gatewayPort, serviceId, clusterId);

            if (usingSerilog)
            {
                clientBuilder.ConfigureLogging(builder => { builder.AddSerilog(dispose: true); });
            }

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
