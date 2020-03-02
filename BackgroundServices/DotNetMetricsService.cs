using App.Metrics;
using App.Metrics.DotNetRuntime;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics.BackgroundServices
{
    public sealed class DotNetMetricsService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly DotNetRuntimeStatsCollector _dotNetRuntimeStatsCollector;

        public DotNetMetricsService(IMetrics metrics, ILogger<DotNetMetricsService> logger)
        {
            _logger = logger;
            _dotNetRuntimeStatsCollector = DotNetRuntimeStatsBuilder.Default(metrics).StartCollecting();
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose()
        {
            _dotNetRuntimeStatsCollector?.Dispose();
        }
    }
}