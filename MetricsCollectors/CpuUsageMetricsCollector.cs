using App.Metrics;
using App.Metrics.Histogram;

using Aragas.QServer.Metrics.BackgroundServices;

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class CpuUsageMetricsCollector : BaseMetricsCollector
    {
        private readonly HistogramOptions process_cpu_usage_percent = new HistogramOptions()
        {
            Name = "cpu_usage_percent",
            Context = "process",
            MeasurementUnit = Unit.Percent
        };

        private readonly ICpuUsageMonitor _cpuUsageMonitor;
        private readonly ILogger _logger;

        public CpuUsageMetricsCollector(IMetrics metrics, ICpuUsageMonitor cpuUsageMonitor, ILogger<CpuUsageMetricsCollector> logger) : base(metrics)
        {
            _cpuUsageMonitor = cpuUsageMonitor;
            _logger = logger;
        }

        public override ValueTask UpdateAsync(CancellationToken stoppingToken)
        {
            Metrics.Measure.Histogram.Update(process_cpu_usage_percent, (long) _cpuUsageMonitor.CpuUsagePercent * 100 * 100);

            return default;
        }

        public override void Dispose() { }
    }
}