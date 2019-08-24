using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Polly;
using Polly.Retry;
// ReSharper disable UnusedMember.Global

namespace GranDen.Orleans.Client.CommonLib
{
    /// <summary>
    /// Orleans Client connect utility
    /// </summary>
    public static class OrleansClientConnectExtension
    {
        /// <summary>
        /// Make Orleans client do connect with default exponential back off retry policy.
        /// </summary>
        /// <param name="client">The Orleans client build from <c>OrleansClientBuilder</c></param>
        /// <param name="cancellationToken">Stop trying to connect token</param>
        /// <param name="retryCount">Retry count, default is 5.</param>
        /// <param name="policy">Optional, default will be exponential back off + jitter retry policy.</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static Task ConnectWithRetryAsync(this IClusterClient client, CancellationToken cancellationToken, int retryCount = 5, AsyncRetryPolicy policy = null, ILogger logger = null)
        {
            var retryPolicy = policy;
            if (retryPolicy == null)
            {
                var random = new Random();
                retryPolicy = CreateRetryPolicy(random, retryCount);
            }

            return retryPolicy.ExecuteAsync(ct => client.Connect((ex) =>
            {
                logger?.LogDebug(ex, "Jitter error occurred");

                return Task.FromResult(!ct.IsCancellationRequested);
            }), cancellationToken);
        }

        /// <summary>
        /// Make Orleans client do connect with default exponential back off retry policy.
        /// </summary>
        /// <param name="client">The Orleans client build from <c>OrleansClientBuilder</c></param>
        /// <param name="retryCount">Retry count, default is 5.</param>
        /// <param name="policy">Optional, default will be exponential back off + jitter retry policy.</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static Task ConnectWithRetryAsync(this IClusterClient client, int retryCount = 5, AsyncRetryPolicy policy = null, ILogger logger = null)
        {
            var retryPolicy = policy;
            if (retryPolicy == null)
            {
                var random = new Random();
                retryPolicy = CreateRetryPolicy(random, retryCount);
            }

            return retryPolicy.ExecuteAsync(() => client.Connect((ex) => 
            {
                logger?.LogDebug(ex, "Jitter error occurred");

                return Task.FromResult(true);
            }));
        }

        #region Private Methods

        private static AsyncRetryPolicy CreateRetryPolicy(Random random, int retryCount)
        {
            // use exponential back off + jitter strategy to the retry policy
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
            return Policy.Handle<SiloUnavailableException>()
                .WaitAndRetryAsync(retryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(random.Next(0, 100)));
        }

        #endregion
    }
}
