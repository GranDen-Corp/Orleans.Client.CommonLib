using GranDen.Orleans.Client.CommonLib.TypedOptions;
using Microsoft.Extensions.Configuration;
// ReSharper disable UnusedMember.Global

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

            return config.GetSiloSettings(orleansSiloClusterSectionKey, orleansSiloProviderSectionKey);
        }

        /// <summary>
        /// Get client typed config object helper method
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="orleansSiloClusterSectionKey">Cluster section key, default will be "Cluster"</param>
        /// <param name="orleansSiloProviderSectionKey">Storage section key, default will be "Provider"</param>
        /// <returns></returns>
        public static (ClusterInfoOption, OrleansProviderOption) GetSiloSettings(this IConfiguration configuration,
                                string orleansSiloClusterSectionKey = "Cluster",
                                string orleansSiloProviderSectionKey = "Provider")
        {
            var clusterInfo = configuration.GetOptionObject<ClusterInfoOption>(orleansSiloClusterSectionKey);
            var providerOption = configuration.GetOptionObject<OrleansProviderOption>(orleansSiloProviderSectionKey);

            return (clusterInfo, providerOption);
        }

        /// <summary>
        /// Bind Typed Option Class from .NET Core's Configuration
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="sectionKey"></param>
        /// <returns></returns>
        public static TObj GetOptionObject<TObj>(this IConfiguration configuration, string sectionKey) where TObj : new()
        {
            var retObj = new TObj();
            configuration.GetSection(sectionKey).Bind(retObj);
            return retObj;
        }
    }
}
