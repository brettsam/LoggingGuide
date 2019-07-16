using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace LoggingGuide
{
    public class ConsoleColorLogger : ILogger
    {
        private readonly ConsoleColor _consoleColor;
        private readonly string _category;
        private readonly IExternalScopeProvider _scopeProvider;

        public ConsoleColorLogger(string category, ConsoleColor color)
            : this(category, color, null)
        {
        }

        public ConsoleColorLogger(string category, ConsoleColor color, IExternalScopeProvider scopeProvider)
        {
            _consoleColor = color;
            _category = category;
            _scopeProvider = scopeProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = _consoleColor;

            Console.WriteLine();
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Category:  {_category}");
            Console.WriteLine($"Level:     {logLevel}");
            Console.WriteLine($"Formatter: {formatter(state, exception)}");
            Console.WriteLine("State:");

            // Require state to be key/value pairs. Otherwise, ignore.
            if (state is IEnumerable<KeyValuePair<string, object>> stateKvps)
            {
                foreach (var kvp in stateKvps)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }

            if (_scopeProvider != null)
            {
                Console.WriteLine("Scope:");
                {
                    _scopeProvider.ForEachScope<object>((s, _) =>
                    {
                        if (s is IEnumerable<KeyValuePair<string, object>> kvps)
                        {
                            foreach (var kvp in kvps.Where(p => p.Key != "{OriginalFormat}"))
                            {
                                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                            }
                        }
                    }, null);
                }
            }

            Console.ForegroundColor = previousColor;
        }
    }
}
