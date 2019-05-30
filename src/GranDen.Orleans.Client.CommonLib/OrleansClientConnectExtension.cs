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
        /// Make Orleans client do connect with default exponential backoff retry policy.
        /// </summary>
        /// <param name="client">The Orleans client build from <c>OrleansClientBuilder</c></param>
        /// <param name="policy">Optional, default will be exponential backoff + jitter retry policy.</param>
        /// <returns></returns>
        public static Task ConnectWithRetryAsync(this IClusterClient client, AsyncRetryPolicy policy = null )
        {
            var retryPolicy = policy;
            if (retryPolicy == null)
            {
                // use exponential backoff + jitter strategy to the retry policy
                // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
                Random jitterer = new Random();
                retryPolicy = Policy.Handle<SiloUnavailableException>().WaitAndRetryAsync(5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))+ TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
            }

            return retryPolicy.ExecuteAsync(async () => { await client.Connect(); });
        }
    }
}
