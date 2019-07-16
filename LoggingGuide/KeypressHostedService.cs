using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoggingGuide
{
    internal class KeypressHostedService : IHostedService
    {
        private readonly ILogger<KeypressHostedService> _logger;
        private Task _readKeyTask;
        private bool _exit = false;

        public KeypressHostedService(ILogger<KeypressHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _readKeyTask = ReadKeyAsync();
            return Task.CompletedTask;
        }

        public async Task ReadKeyAsync()
        {
            await Task.Yield();

            while (!_exit)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    switch (key.Key.ToString())
                    {
                        case "E":
                            _logger.LogError("Error log.");
                            break;
                        case "D":
                            _logger.LogDebug("Debug log.");
                            break;
                        case "C":
                            _logger.LogCritical("Critical log.");
                            break;
                        case "W":
                            _logger.LogWarning("Warning log.");
                            break;
                        case "T":
                            _logger.LogTrace("Trace log.");
                            break;
                        case "L":
                            WriteLogWithLog("Log!");
                            break;
                        case "R":
                            _logger.LogInformation("Structured. The time is '{Time}'. A guid is '{Guid}'.", DateTime.Now, Guid.NewGuid());
                            break;
                        case "S":
                            using (_logger.BeginScope("{Key1}", "A"))
                            {
                                await DoSomething1Async();
                            }
                            break;
                        default:
                            _logger.LogInformation($"`{key.Key}` pressed.");
                            break;
                    }
                }

            }
        }

        private void WriteLogWithLog(string log)
        {
            var stateDict = new Dictionary<string, object>
            {
                {"Key1", true },
                { "Key2", "ABC" }
            };

            _logger.Log(LogLevel.Information, 123, stateDict, null, (state, ex) => $"The state has {state.Count} items.");
        }

        private async Task DoSomething1Async()
        {
            await Task.Delay(1);

            using (_logger.BeginScope("{Key2}", "B"))
            {
                await DoSomething2();
            }
        }

        private async Task DoSomething2()
        {
            using (_logger.BeginScope("{Key1} value is {Key2}", "C", "D"))
            {
                await DoSomething3Async();
            }
        }

        private async Task DoSomething3Async()
        {
            await Task.Delay(1);

            _logger.LogInformation("Logging with scope.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _exit = true;
            await _readKeyTask;
        }
    }
}