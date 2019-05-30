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
        /// <param name="orleansSecetionKey"></param>
        /// <param name="orleansSiloClusterSectionKey"></param>
        /// <param name="orleansSiloProviderSectionKey"></param>
        /// <returns></returns>
        public static (ClusterInfoOption, OrleansProviderOption) GetSiloSettings(this IConfigurationRoot configurationRoot, 
                                string orleansSecetionKey = "Orleans", 
                                string orleansSiloClusterSectionKey = "Cluster", 
                                string orleansSiloProviderSectionKey = "Provider")
        {
           var config = configurationRoot.GetSection(orleansSecetionKey);

            var clusterInfo = new ClusterInfoOption();
            config.GetSection(orleansSiloClusterSectionKey).Bind(clusterInfo);

            var providerOption = new OrleansProviderOption();
            config.GetSection(orleansSiloProviderSectionKey).Bind(providerOption);

            return (clusterInfo, providerOption);
        }
    }
}
