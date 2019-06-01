using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GranDen.Orleans.Client.CommonLib.TypedOptions;
using Microsoft.Extensions.Configuration;

namespace GranDen.Orleans.Client.CommonLib
{
    /// <summary>
    /// Get Client configuration object utility
    /// </summary>
    public static class OrleansClientConfigurationExtensions
    {
        /// <summary>
        /// Get client typed config object helper method
        /// </summary>
        /// <param name="configurationRoot"></param>
        /// <param name="orleansSectionKey">Top section key for Orleans setting, default will be "Orleans"</param>
        /// <param name="orleansSiloClusterSectionKey">Cluster section key, default will be "Cluster"</param>
        /// <param name="orleansSiloProviderSectionKey">Storage section key, default will be "Provider"</param>
        /// <returns></returns>
        public static (ClusterInfoOption, OrleansProviderOption) GetSiloSettings(this IConfigurationRoot configurationRoot, 
                                string orleansSectionKey = "Orleans", 
                                string orleansSiloClusterSectionKey = "Cluster", 
                                string orleansSiloProviderSectionKey = "Provider")
        {
           var config = configurationRoot.GetSection(orleansSectionKey);

            var clusterInfo = new ClusterInfoOption();
            config.GetSection(orleansSiloClusterSectionKey).Bind(clusterInfo);

            var providerOption = new OrleansProviderOption();
            config.GetSection(orleansSiloProviderSectionKey).Bind(providerOption);

            return (clusterInfo, providerOption);
        }
    }
}
