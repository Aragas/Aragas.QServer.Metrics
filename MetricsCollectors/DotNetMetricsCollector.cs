using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;

using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class DotNetMetricsCollector : BaseMetricsCollector
    {
        private readonly GaugeOptions dotnet_total_memory_bytes = new GaugeOptions()
        {
            Name = "total_memory_bytes",
            Context = "dotnet",
            MeasurementUnit = Unit.Bytes
        };
        private readonly CounterOptions dotnet_collection_count = new CounterOptions()
        {
            Name = "collection_count",
            Context = "dotnet",
            MeasurementUnit = Unit.Items
        };

        private readonly ILogger _logger;

        public DotNetMetricsCollector(IMetrics metrics, ILogger<DotNetMetricsCollector> logger) : base(metrics)
        {
            _logger = logger;
        }

        public override ValueTask UpdateAsync(CancellationToken stoppingToken)
        {
            for (var gen = 0; gen <= GC.MaxGeneration; gen++)
                Metrics.Measure.Counter.Increment(dotnet_collection_count, GC.CollectionCount(gen), $"gen {gen}");

            Metrics.Measure.Gauge.SetValue(dotnet_total_memory_bytes, GC.GetTotalMemory(false));

            return default;
        }

        public override void Dispose() { }
    }
}