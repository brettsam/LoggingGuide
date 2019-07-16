using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LoggingWebJobs
{
    public class TimerFunction
    {
        /// <summary>
        /// This ILogger is *not* injected by DI. It's actually a WebJobs binding.
        /// - https://github.com/Azure/azure-webjobs-sdk/blob/dev/src/Microsoft.Azure.WebJobs.Host/Bindings/ILogger/ILoggerBinding.cs#L46
        /// Host.Results
        /// Host.Aggregator
        /// </summary>
        public void BlobTrigger(
            [TimerTrigger("*/5 * * * * *")] TimerInfo timerInfo, ILogger logger)
        {
            logger.LogInformation("Timer fired.");
        }
    }
}
