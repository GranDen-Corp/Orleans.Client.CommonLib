using System.Threading.Tasks;
using HelloNetStandard2.ShareInterface;
using Microsoft.Extensions.Logging;

namespace HelloNetStandard2.Grains
{
    public class HelloGrain : Orleans.Grain, IHello
    {
        private readonly ILogger<HelloGrain> _logger;

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            _logger = logger;
        }

        public Task<string> SayHello(string greeting)
        {
            _logger.LogInformation($"SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"You said: '{greeting}', I say: Hello!");
        }
    }
}