using System;
using Microsoft.Extensions.Logging;

namespace LoggingGuide
{
    [ProviderAlias("Green")]
    public class GreenLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private IExternalScopeProvider _scopeProvider;

        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleColorLogger(categoryName, ConsoleColor.Green, _scopeProvider);
        }

        public void Dispose()
        {
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }
    }
}
