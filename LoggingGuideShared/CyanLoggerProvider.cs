using System;
using Microsoft.Extensions.Logging;

namespace LoggingGuide
{
    public class CyanLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleColorLogger(categoryName, ConsoleColor.Cyan);
        }

        public void Dispose()
        {
        }
    }
}
