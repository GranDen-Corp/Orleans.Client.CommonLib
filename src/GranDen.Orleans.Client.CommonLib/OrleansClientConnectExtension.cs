using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Polly;
using Polly.Retry;

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
        /// <param name="retryCount">Retry count, default is 5.</param>
        /// <param name="policy">Optional, default will be exponential back off + jitter retry policy.</param>
        /// <returns></returns>
        public static Task ConnectWithRetryAsync(this IClusterClient client, int retryCount = 5,  AsyncRetryPolicy policy = null)
        {
            var retryPolicy = policy;
            if (retryPolicy == null)
            {
                // use exponential back off + jitter strategy to the retry policy
                // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
                Random random = new Random();
                retryPolicy = Policy.Handle<SiloUnavailableException>()
                    .WaitAndRetryAsync(retryCount, retryAttempt =>
                         TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(random.Next(0, 100)));
            }

            return retryPolicy.ExecuteAsync(async () => { await client.Connect(); });
        }
    }
}
