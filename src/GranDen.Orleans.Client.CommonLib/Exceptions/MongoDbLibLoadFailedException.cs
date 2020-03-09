using System;

namespace GranDen.Orleans.Client.CommonLib.Exceptions
{
    /// <summary>
    /// Exception for situation when Mongo DB clustering library not found or initiated failed.
    /// </summary>
    public class MongoDbLibLoadFailedException : OrleansLibLoadFailedException
    {
        /// <summary>
        /// Raise when associated Mongo DB hosting configuration library not exist or initiated failed in applied project.
        /// </summary>
        /// <param name="innerException"></param>
        public MongoDbLibLoadFailedException(Exception innerException) : base(innerException)
        {
        }

        /// <inheritdoc />
        public sealed override string LibNugetName { get; protected set; } = "Orleans.Providers.MongoDB";
        /// <inheritdoc />
        public sealed override string LibPurpose { get; protected set; } = "mongodb clustering";
    }
}
