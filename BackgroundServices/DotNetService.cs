using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;

namespace App.Metrics.DotNetRuntime.BackgroundServices
{
    public class DotNetService : IHostedService
    {
        private readonly IMetrics _metrics;
        private readonly ILogger _logger;
        private DotNetRuntimeStatsCollector? _dotNetRuntimeStatsCollector;

        public DotNetService(IMetrics metrics, ILogger<DotNetService> logger)
        {
            _metrics = metrics;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dotNetRuntimeStatsCollector = DotNetRuntimeStatsBuilder.Default(_metrics).StartCollecting();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _dotNetRuntimeStatsCollector?.Dispose();
            return Task.CompletedTask;
        }
    }
}