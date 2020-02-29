using Aragas.QServer.Metrics;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace App.Metrics.DotNetRuntime.BackgroundServices
{
    internal class MetricsCollectorService : BackgroundService
    {
        private readonly List<IMetricsCollector> _metricsCollectors;

        private readonly IMetrics _metrics;
        private readonly ILogger _logger;
        private readonly int _delay;

        public MetricsCollectorService(IMetrics metrics, ILogger<MetricsCollectorService> logger, List<IMetricsCollector> metricsCollectors, int delay = 3000)
        {
            _metrics = metrics;
            _logger = logger;
            _metricsCollectors = metricsCollectors;
            _delay = delay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting reporting. Delay:{Delay}", GetType().Name, _delay);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var metricsCollector in _metricsCollectors)
                    metricsCollector.UpdateMetrics(_metrics);

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
    }
}