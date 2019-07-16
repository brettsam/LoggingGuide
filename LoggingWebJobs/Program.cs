using System;
using System.Threading.Tasks;
using LoggingGuide;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoggingWebJobs
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddTimers();
                })
                .ConfigureServices(s =>
                {
                    s.Configure<FunctionResultAggregatorOptions>(o => o.FlushTimeout = TimeSpan.FromSeconds(10));
                })
                .ConfigureLogging((context, b) =>
                {
                    //b.AddConsole();
                    b.AddProvider(new GreenLoggerProvider());
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
