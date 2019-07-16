using LoggingFunctions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

[assembly: FunctionsStartup(typeof(Startup))]

namespace LoggingFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"c:\users\brettsam\desktop\log.txt")
                .CreateLogger();

            builder.Services.AddLogging(b =>
            {
                b.AddSerilog(dispose: true);
            });
        }
    }
}
