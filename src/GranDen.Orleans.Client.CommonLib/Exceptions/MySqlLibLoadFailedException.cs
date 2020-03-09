using System;

namespace GranDen.Orleans.Client.CommonLib.Exceptions
{
    /// <summary>
    /// Exception for situation when MySQL DB clustering library not found or initiated failed.
    /// </summary>
    public class MySqlLibLoadFailedException : OrleansLibLoadFailedException
    {
        /// <summary>
        /// Raise when associated MySQL DB hosting configuration library not exist or initiated failed in applied project.
        /// </summary>
        /// <param name="innerException"></param>
        public MySqlLibLoadFailedException(Exception innerException) : base(innerException)
        {
        }

        /// <inheritdoc />
        public sealed override string LibNugetName { get; protected set; } = "Microsoft.Orleans.Clustering.AdoNet & MySql.Data";
        /// <inheritdoc />
        public sealed override string LibPurpose { get; protected set; } = "MySQL db clustering";
    }
}
