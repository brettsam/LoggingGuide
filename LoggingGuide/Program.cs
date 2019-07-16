using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoggingGuide
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Enter scenario number: ");
            int scenario = Int32.Parse(Console.ReadLine());

            IHostBuilder builder = null;

            switch (scenario)
            {
                case 1:
                    builder = CreateSimpleLogger();
                    break;
                case 2:
                    builder = CreateSimpleLoggerWithScope();
                    break;
                case 3:
                    builder = CreateLoggerWithFilteringInCode();
                    break;
                case 4:
                    builder = CreateLoggerWithFilteringInConfiguration();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            IHost host = builder.Build();

            using (host)
            {
                await host.RunAsync();
            }
        }

        /// <summary>
        /// Adds a simple <see cref="CyanLoggerProvider"/> to the host.
        /// - <see cref="ILogger"/> / <see cref="ILoggerProvider"/> / <see cref="ILoggerFactory"/> interfaces
        /// - Logger provider registration.
        ///   - ConfigureLogging(): https://github.com/aspnet/Extensions/blob/master/src/Hosting/Hosting/src/HostingHostBuilderExtensions.cs#L83
        ///   - AddLogging(): https://github.com/aspnet/Extensions/blob/master/src/Logging/Logging/src/LoggingServiceCollectionExtensions.cs#L32
        ///   - Logger<>: https://github.com/aspnet/Extensions/blob/master/src/Logging/Logging.Abstractions/src/LoggerT.cs
        /// - Structured logging
        /// </summary>
        private static IHostBuilder CreateSimpleLogger()
        {
            Console.WriteLine($"...building host with simple {nameof(CyanLoggerProvider)}.");
            Console.WriteLine();

            return CreateBaseBuilder()
                .ConfigureLogging(b =>
                {
                    b.AddProvider(new CyanLoggerProvider());
                });
        }

        /// <summary>
        /// Adds a <see cref="GreenLoggerProvider"/> to the host, using scopes.
        /// - AsyncLocal: https://docs.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1?view=netcore-2.2
        /// - DictionaryLoggerScope: https://github.com/Azure/azure-webjobs-sdk/blob/dev/src/Microsoft.Azure.WebJobs.Logging.ApplicationInsights/DictionaryLoggerScope.cs
        /// - IExternalScopeProvider.
        /// </summary>
        private static IHostBuilder CreateSimpleLoggerWithScope()
        {
            Console.WriteLine($"...building host with simple {nameof(GreenLoggerProvider)}, using scopes. Press 's' to see scopes.");
            Console.WriteLine();

            return CreateBaseBuilder()
                .ConfigureLogging(b =>
                {
                    b.Services.AddSingleton<ILoggerProvider, GreenLoggerProvider>();
                });
        }

        /// <summary>
        /// Adds both loggers and configures filtering in code in different ways.
        /// - Filter in code: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.2#filter-rules-in-code
        /// </summary>
        private static IHostBuilder CreateLoggerWithFilteringInCode()
        {
            Console.WriteLine("...building host with two filtering ILoggerProviders. Press 't' for Trace, 'd' for Debug, 'i' for Information, 'w' for Warning, 'e' for Error, 'c' for Critical.");
            Console.WriteLine();

            return CreateBaseBuilder()
                .ConfigureLogging((c, b) =>
                {
                    b.Services.AddSingleton<ILoggerProvider, GreenLoggerProvider>();
                    b.Services.AddSingleton<ILoggerProvider, CyanLoggerProvider>();

                    b.SetMinimumLevel(LogLevel.Warning);

                    // Applies to all ILoggerProviders.
                    b.AddFilter((category, level) => true);

                    // Applies only to Cyan; all categories
                    b.AddFilter<CyanLoggerProvider>(null, LogLevel.Critical);

                    // Applies only to Green; only categories starting with "LoggingGuide"
                    b.AddFilter<GreenLoggerProvider>("LoggingGuide", LogLevel.Debug);
                });
        }


        /// <summary>
        /// Adds both loggers and configures filtering in configuration.
        /// - Filter in configuration: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.2#create-filter-rules-in-configuration
        /// </summary>
        private static IHostBuilder CreateLoggerWithFilteringInConfiguration()
        {
            Console.WriteLine("...building host with two filtering ILoggerProviders. Press 't' for Trace, 'd' for Debug, 'i' for Information, 'w' for Warning, 'e' for Error, 'c' for Critical.");
            Console.WriteLine();

            return CreateBaseBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("appsettings.json", optional: false);

                })
                .ConfigureLogging((c, b) =>
                {
                    b.AddConfiguration(c.Configuration.GetSection("Logging"));

                    b.Services.AddSingleton<ILoggerProvider, GreenLoggerProvider>();
                    b.Services.AddSingleton<ILoggerProvider, CyanLoggerProvider>();
                });
        }

        private static IHostBuilder CreateBaseBuilder()
        {
            return new HostBuilder()
                .ConfigureServices(s =>
                {
                    s.AddSingleton<IHostedService, KeypressHostedService>();
                    s.Configure<ConsoleLifetimeOptions>(o => o.SuppressStatusMessages = true);
                });
        }
    }
}
