using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using Orleans.Configuration;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Orleans;
using Orleans.Hosting;


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
            try
            {
                return CreateClientBuilder(logger, clusterInfo, providerOption, applicationPartTypes, usingSerilog).Build();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create Silo Client failed");
                throw;
            }
        }

        /// <summary>
        /// Initialize a normal Orleans ClientBuilder
        /// </summary>
        /// <param name="logger">Logger to log ClientBuilder operation information</param>
        /// <param name="clusterInfo"></param>
        /// <param name="providerOption"></param>
        /// <param name="applicationPartTypes">Application parts (optional)</param>
        /// <param name="usingSerilog">Default use Serilog (https://serilog.net), set to <c>false</c> to use Orleans' original logging.</param>
        /// <returns></returns>
        public static IClientBuilder CreateClientBuilder(ILogger logger,
            ClusterInfoOption clusterInfo,
            OrleansProviderOption providerOption,
            IEnumerable<Type> applicationPartTypes = null,
            bool usingSerilog = true)
        {
            var clientBuilder = new ClientBuilder()
                .ConfigureCluster(clusterInfo, TimeSpan.FromSeconds(20), TimeSpan.FromMinutes(60));
            if(providerOption.DefaultProvider.ToLower() == "sqldb")
            {
                logger.LogTrace("Using SQL DB provider");
                clientBuilder.UseAdoNetClustering(options =>
                {
                    var sqlDbSetting = providerOption.SQLDB.Cluster;

                    options.Invariant = sqlDbSetting.Invariant;
                    options.ConnectionString = sqlDbSetting.DbConn;
                });

            }
            else if (providerOption.DefaultProvider.ToLower() == "mongodb")
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

            return clientBuilder;
        }

        /// <summary>
        /// Create Orleans Client using static route option
        /// </summary>
        /// <param name="logger">Logger to log ClientBuilder operation information</param>
        /// <param name="clusterInfo"></param>
        /// <param name="staticGatewayOption"></param>
        /// <param name="applicationPartTypes">Application parts (optional)</param>
        /// <param name="usingSerilog">Default use Serilog (https://serilog.net), set to <c>false</c> to use Orleans' original logging.</param>
        /// <returns></returns>
        public static IClusterClient CreateStaticRouteClient(ILogger logger,
            ClusterInfoOption clusterInfo,
            StaticGatewayListProviderOptions staticGatewayOption,
            IEnumerable<Type> applicationPartTypes = null,
            bool usingSerilog = true)
        {
            try
            {
                return CreateStaticRouteClientBuilder(clusterInfo, staticGatewayOption, applicationPartTypes, usingSerilog).Build();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Creat Silo Client failed");
                throw;
            }
        }

        /// <summary>
        /// Initialize Static Route ClientBuilder
        /// </summary>
        /// <param name="clusterInfo"></param>
        /// <param name="staticGatewayOption"></param>
        /// <param name="applicationPartTypes"></param>
        /// <param name="usingSerilog"></param>
        /// <returns></returns>
        public static IClientBuilder CreateStaticRouteClientBuilder(
            ClusterInfoOption clusterInfo,
            StaticGatewayListProviderOptions staticGatewayOption,
            IEnumerable<Type> applicationPartTypes = null,
            bool usingSerilog = true)
        {
            var clientBuilder = new ClientBuilder();
            clientBuilder.ConfigureCluster(clusterInfo, TimeSpan.FromSeconds(20), TimeSpan.FromMinutes(60));
            clientBuilder.UseStaticClustering(option =>
                {
                    option.Gateways = staticGatewayOption.Gateways;
                });

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

            return clientBuilder;
        }

        private static ClientBuilder ConfigureCluster(this ClientBuilder clientBuilder,
            ClusterInfoOption clusterInfo, TimeSpan responseTimeout, TimeSpan responseTimeoutWithDebugger)
        {
            clientBuilder.Configure<ClientMessagingOptions>(options =>
            {
                options.ResponseTimeout = responseTimeout;
                options.ResponseTimeoutWithDebugger = responseTimeoutWithDebugger;
            }).Configure<ClusterOptions>(options =>
            {
                options.ClusterId = clusterInfo.ClusterId;
                options.ServiceId = clusterInfo.ServiceId;
            });

            return clientBuilder;
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
