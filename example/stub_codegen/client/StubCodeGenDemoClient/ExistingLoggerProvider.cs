using Microsoft.Extensions.Logging;

namespace StubCodeGenDemoClient
{
    internal class ExistingLoggerProvider<T> : ILoggerProvider
    {
        private readonly ILogger<T> logger;

        public ExistingLoggerProvider(ILogger<T> logger)
        {
            this.logger = logger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return logger;
        }

        public void Dispose()
        {
            //do nothing
        }
    }
}