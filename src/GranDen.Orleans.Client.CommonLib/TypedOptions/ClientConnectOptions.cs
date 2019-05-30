using System.ComponentModel.DataAnnotations;

namespace GranDen.Orleans.Client.CommonLib.TypedOptions
{
    /// <summary>
    /// Orleans Silo typed configuration class
    /// </summary>
    public class ClusterInfoOption
    {
        [Required]
        public string ClusterId { get; set; }
        [Required]
        public string ServiceId { get; set; }
    }

    /// <summary>
    /// Default Storage Provider's name, currently only "MongoDB"
    /// </summary>
    public class OrleansProviderOption
    {
        [Required]
        public string DefaultProvider { get; set; }

        public MongoDbProviderSettings MongoDB { get; set; }
    }

    /// <summary>
    /// MongoDB Provider typed configuration class
    /// </summary>
    public class MongoDbProviderSettings
    {
        public MongoDbProviderClusterSettings Cluster { get; set; }
    }

    /// <summary>
    /// Silo Cluster MongoDB Provider typed configuration class
    /// </summary>
    public class MongoDbProviderClusterSettings
    {
        [Required]
        public string DbConn { get; set; }
        [Required]
        public string DbName { get; set; }
        public string CollectionPrefix { get; set; }
    }
}
