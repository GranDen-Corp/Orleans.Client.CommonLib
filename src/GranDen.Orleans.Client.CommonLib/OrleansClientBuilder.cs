using System;
using System.Collections.Generic;
using GranDen.CallExtMethodLib;
using GranDen.Orleans.Client.CommonLib.Exceptions;
using Microsoft.Extensions.Logging;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using Orleans.Configuration;
using Orleans;
using Orleans.Runtime.Configuration;

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
        /// <param name="configureLogging"></param>
        /// <returns></returns>
        public static IClusterClient CreateClient(ILogger logger,
            ClusterInfoOption clusterInfo,
            OrleansProviderOption providerOption,
            IEnumerable<Type> applicationPartTypes = null,
            Action<ILoggingBuilder> configureLogging = null)
        {
            try
            {
                var builder = CreateClientBuilder(logger, clusterInfo, providerOption, applicationPartTypes);
                if (configureLogging != null)
                {
                    builder.ConfigureLogging(configureLogging);
                }
                return builder.Build();
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
        /// <returns></returns>
        public static IClientBuilder CreateClientBuilder(ILogger logger,
            ClusterInfoOption clusterInfo,
            OrleansProviderOption providerOption,
            IEnumerable<Type> applicationPartTypes = null)
        {
            var clientBuilder = new ClientBuilder()
                .ConfigureCluster(clusterInfo, TimeSpan.FromSeconds(20), TimeSpan.FromMinutes(60))
                .Configure<StatisticsOptions>(options =>
                {
                    options.CollectionLevel = StatisticsLevel.Critical;
                });

            switch (providerOption.DefaultProvider.ToLower())
            {
                case "sqldb":
                    {

                        logger.LogTrace("Using SQL DB provider");
                        var sqlDbSetting = providerOption.SQLDB.Cluster;
                        try
                        {
                            var helper = new ExtMethodInvoker("Orleans.Clustering.AdoNet");
                            var adoNetClusteringClientOptionsType = helper.ExtensionLibAssembly.GetType("Orleans.Configuration.AdoNetClusteringClientOptions", true);
                            var adoNetClusteringClientOptionsValue = new Dictionary<string, object>
                            {
                                ["ConnectionString"] = sqlDbSetting.DbConn,
                                ["Invariant"] = sqlDbSetting.Invariant ?? @"System.Data.SqlClient"
                            };
                            var configSqlDbClusteringAction =
                                CreateDelegateHelper.CreateAssignValueAction(adoNetClusteringClientOptionsType, "options", adoNetClusteringClientOptionsValue);

                            clientBuilder = helper.Invoke<IClientBuilder>(
                                new ExtMethodInfo { MethodName = "UseAdoNetClustering", ExtendedType = typeof(IClientBuilder) },
                                clientBuilder, configSqlDbClusteringAction);
                        }
                        catch (Exception ex)
                        {
                            throw new SqlDbLibLoadFailedException(ex);
                        }
                    }
                    break;

                case "mysql":
                    {
                        logger.LogTrace("Using MySQL DB provider");
                        var mysqlDbSetting = providerOption.SQLDB.Cluster;
                        try
                        {
                            var helper = new ExtMethodInvoker("Orleans.Clustering.AdoNet");
                            var adoNetClusteringClientOptionsType = helper.ExtensionLibAssembly.GetType("Orleans.Configuration.AdoNetClusteringClientOptions", true);
                            var adoNetClusteringClientOptionsValue = new Dictionary<string, object>
                            {
                                ["ConnectionString"] = mysqlDbSetting.DbConn,
                                ["Invariant"] = mysqlDbSetting.Invariant ?? @"MySql.Data.MySqlClient"
                            };
                            var configSqlDbClusteringAction =
                                CreateDelegateHelper.CreateAssignValueAction(adoNetClusteringClientOptionsType, "options", adoNetClusteringClientOptionsValue);

                            clientBuilder = helper.Invoke<IClientBuilder>(
                                new ExtMethodInfo { MethodName = "UseAdoNetClustering", ExtendedType = typeof(IClientBuilder) },
                                clientBuilder, configSqlDbClusteringAction);
                        }
                        catch (Exception ex)
                        {
                            throw new MySqlLibLoadFailedException(ex);
                        }
                    }
                    break;

                case "mongodb":
                    {
                        logger.LogTrace("Using MongoDB provider...");
                        var mongoSetting = providerOption.MongoDB.Cluster;
                        try
                        {
                            var helper = new ExtMethodInvoker("Orleans.Providers.MongoDB");
                            clientBuilder = helper.Invoke<IClientBuilder>(
                                new ExtMethodInfo { MethodName = "UseMongoDBClient", ExtendedType = typeof(IClientBuilder) },
                                clientBuilder, mongoSetting.DbConn);

                            var mongoDbMembershipTableOptionsType =
                                helper.ExtensionLibAssembly.GetType("Orleans.Providers.MongoDB.Configuration.MongoDBGatewayListProviderOptions", true);
                            var mongoDbMembershipTableOptionsValue = new Dictionary<string, object>
                            {
                                ["DatabaseName"] = mongoSetting.DbName
                            };
                            if (!string.IsNullOrEmpty(mongoSetting.CollectionPrefix))
                            {
                                mongoDbMembershipTableOptionsValue["CollectionPrefix"] = mongoSetting.CollectionPrefix;
                            }
                            var configMongoDbClusteringAction =
                                CreateDelegateHelper.CreateAssignValueAction(mongoDbMembershipTableOptionsType, "options", mongoDbMembershipTableOptionsValue);

                            clientBuilder = helper.Invoke<IClientBuilder>(
                                new ExtMethodInfo { MethodName = "UseMongoDBClustering", ExtendedType = typeof(IClientBuilder) },
                                clientBuilder, configMongoDbClusteringAction);
                        }
                        catch (Exception ex)
                        {
                            throw new MongoDbLibLoadFailedException(ex);
                        }
                    }
                    break;
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

            return clientBuilder;
        }

        /// <summary>
        /// Create Orleans Client using static route option
        /// </summary>
        /// <param name="logger">Logger to log ClientBuilder operation information</param>
        /// <param name="clusterInfo"></param>
        /// <param name="staticGatewayOption"></param>
        /// <param name="applicationPartTypes">Application parts (optional)</param>
        /// <param name="configureLogging"></param>
        /// <returns></returns>
        public static IClusterClient CreateStaticRouteClient(ILogger logger,
            ClusterInfoOption clusterInfo,
            StaticGatewayListProviderOptions staticGatewayOption,
            IEnumerable<Type> applicationPartTypes = null,
            Action<ILoggingBuilder> configureLogging = null)
        {
            try
            {
                var builder = CreateStaticRouteClientBuilder(clusterInfo, staticGatewayOption, applicationPartTypes);
                if (configureLogging != null)
                {
                    builder.ConfigureLogging(configureLogging);
                }

                return builder.Build();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create Silo Client failed");
                throw;
            }
        }

        /// <summary>
        /// Initialize Static Route ClientBuilder
        /// </summary>
        /// <param name="clusterInfo"></param>
        /// <param name="staticGatewayOption"></param>
        /// <param name="applicationPartTypes">Application parts (optional)</param>
        /// <returns></returns>
        public static IClientBuilder CreateStaticRouteClientBuilder(
            ClusterInfoOption clusterInfo,
            StaticGatewayListProviderOptions staticGatewayOption,
            IEnumerable<Type> applicationPartTypes = null)
        {
            var clientBuilder = new ClientBuilder()
                .ConfigureCluster(clusterInfo, TimeSpan.FromSeconds(20), TimeSpan.FromMinutes(60))
                .Configure<StatisticsOptions>(options =>
                {
                    options.CollectionLevel = StatisticsLevel.Critical;
                })
                .UseStaticClustering(option =>
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
        /// Create a local silo connect only client
        /// </summary>
        /// <param name="logger">Logger to log ClientBuilder operation information</param>
        /// <param name="gatewayPort"></param>
        /// <param name="serviceId"></param>
        /// <param name="clusterId"></param>
        /// <param name="applicationPartTypes">Application parts (optional)</param>
        /// <param name="configureLogging"></param>
        /// <returns></returns>
        public static IClusterClient CreateLocalhostClient(ILogger logger,
            int gatewayPort = 30000,
            string clusterId = "dev",
            string serviceId = "dev",
            IEnumerable<Type> applicationPartTypes = null,
            Action<ILoggingBuilder> configureLogging = null)
        {
            try
            {
                var builder = CreateLocalhostClientBuilder(gatewayPort, clusterId, serviceId, applicationPartTypes);

                if (configureLogging != null)
                {
                    builder.ConfigureLogging(configureLogging);
                }
                return builder.Build();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create Silo Client failed");
                throw;
            }
        }

        /// <summary>
        /// Create a local connect only ClientBuilder
        /// </summary>
        /// <param name="gatewayPort"></param>
        /// <param name="serviceId"></param>
        /// <param name="clusterId"></param>
        /// <param name="applicationPartTypes">Application parts (optional)</param>
        /// <returns></returns>
        public static IClientBuilder CreateLocalhostClientBuilder(
        int gatewayPort = 30000,
        string clusterId = "dev",
        string serviceId = "dev",
        IEnumerable<Type> applicationPartTypes = null)
        {
            var clientBuilder = new ClientBuilder()
                .Configure<StatisticsOptions>(options =>
                {
                    options.CollectionLevel = StatisticsLevel.Critical;
                })
                .UseLocalhostClustering(gatewayPort, serviceId, clusterId);

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

            return clientBuilder;
        }
    }
}
