using System.ComponentModel.DataAnnotations;

namespace GranDen.Orleans.Client.CommonLib.TypedOptions
{
    /// <summary>
    /// Orleans Silo typed configuration class
    /// </summary>
    public class ClusterInfoOption
    {
        /// <summary>
        /// Silo Cluster Id
        /// </summary>
        [Required]
        public string ClusterId { get; set; }

        /// <summary>
        /// Silo Service Id
        /// </summary>
        [Required]
        public string ServiceId { get; set; }
    }

    /// <summary>
    /// Default Storage Provider's name, currently only "MongoDB"
    /// </summary>
    public class OrleansProviderOption
    {
        /// <summary>
        /// Default Storage Provider's name, currently only "MongoDB"
        /// </summary>
        [Required]
        public string DefaultProvider { get; set; }

        /// <summary>
        /// Top level MongoDB provider setting
        /// </summary>
        public MongoDbProviderSettings MongoDB { get; set; }
    }

    /// <summary>
    /// MongoDB Provider typed configuration class
    /// </summary>
    public class MongoDbProviderSettings
    {
        /// <summary>
        /// Silo Cluster MongoDB provider setting
        /// </summary>
        public MongoDbProviderClusterSettings Cluster { get; set; }
    }

    /// <summary>
    /// Silo Cluster MongoDB Provider typed configuration class
    /// </summary>
    public class MongoDbProviderClusterSettings
    {
        /// <summary>
        /// MongoDB connection string
        /// </summary>
        [Required]
        public string DbConn { get; set; }

        /// <summary>
        /// MongoDB database name
        /// </summary>
        [Required]
        public string DbName { get; set; }

        /// <summary>
        /// Collection name prefix
        /// </summary>
        public string CollectionPrefix { get; set; }
    }
}
