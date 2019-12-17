using System.Threading.Tasks;
using HelloNetStandard2.ShareInterface;
using Microsoft.Extensions.Logging;

//see: http://dotnet.github.io/orleans/Documentation/grains/code_generation.html#generate-code-for-a-specific-type
[assembly: Orleans.CodeGeneration.KnownType(typeof(IHello))]

namespace NetCore3Demo.Grain
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
