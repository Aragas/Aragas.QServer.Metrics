using App.Metrics;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics.BackgroundServices
{
    public class MetricsCollectorService : BackgroundService, IAsyncDisposable
    {
        private readonly List<BaseMetricsCollector> _metricsCollectors;

        private readonly ILogger _logger;
        private readonly int _delay;

        public MetricsCollectorService(IMetrics metrics, ILogger<MetricsCollectorService> logger, List<BaseMetricsCollector> metricsCollectors, int delay = 3000)
        {
            _logger = logger;
            _metricsCollectors = metricsCollectors;
            _delay = delay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting reporting. Delay: {Delay}", GetType().Name, _delay);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var metricsCollector in _metricsCollectors)
                    await metricsCollector.UpdateAsync(stoppingToken);

                await Task.Delay(_delay, stoppingToken);
            }
        }

        public override void Dispose()
        {
            foreach (var metricsCollector in _metricsCollectors)
                metricsCollector.Dispose();
            _metricsCollectors.Clear();

            base.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var metricsCollector in _metricsCollectors)
                await metricsCollector.DisposeAsync();
            _metricsCollectors.Clear();

            base.Dispose();
        }
    }
}