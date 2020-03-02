using App.Metrics;
using App.Metrics.Histogram;

using Aragas.QServer.Metrics.BackgroundServices;

using Microsoft.Extensions.Logging;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class CpuUsageMetricsCollector : IMetricsCollector
    {
        private readonly HistogramOptions process_cpu_usage_percent = new HistogramOptions()
        {
            Name = "cpu_usage_percent",
            Context = "process",
            MeasurementUnit = Unit.Percent
        };

        private readonly ICpuUsageMonitor _cpuUsageMonitor;
        private readonly ILogger _logger;

        public CpuUsageMetricsCollector(ICpuUsageMonitor cpuUsageMonitor, ILogger<CpuUsageMetricsCollector> logger)
        {
            _cpuUsageMonitor = cpuUsageMonitor;
            _logger = logger;
        }

        public void UpdateMetrics(IMetrics metrics)
        {
            metrics.Measure.Histogram.Update(process_cpu_usage_percent, (long) _cpuUsageMonitor.CpuUsagePercent * 100 * 100);
        }

        public void Dispose() { }
    }
}