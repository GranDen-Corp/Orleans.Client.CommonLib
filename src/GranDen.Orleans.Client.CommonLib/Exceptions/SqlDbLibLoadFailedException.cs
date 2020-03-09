using System;

namespace GranDen.Orleans.Client.CommonLib.Exceptions
{
    /// <summary>
    /// Exception for situation when SQL Server clustering library not found or initiated failed.
    /// </summary>
    public class SqlDbLibLoadFailedException : OrleansLibLoadFailedException
    {
        /// <summary>
        /// Raise when associated SQL Server hosting configuration library not exist or initiated failed in applied project.
        /// </summary>
        /// <param name="innerException"></param>
        public SqlDbLibLoadFailedException(Exception innerException) : base(innerException)
        {
        }

        /// <inheritdoc />
        public sealed override string LibNugetName { get; protected set; } = @"Microsoft.Orleans.Clustering.AdoNet & System.Data.SqlClient / Microsoft.Data.SqlClient";
        /// <inheritdoc />
        public sealed override string LibPurpose { get; protected set; } = "sql db clustering";
    }
}
