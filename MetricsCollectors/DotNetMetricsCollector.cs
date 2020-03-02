using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;

using Microsoft.Extensions.Logging;

using System;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class DotNetMetricsCollector : IMetricsCollector
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

        public DotNetMetricsCollector(ILogger<DotNetMetricsCollector> logger)
        {
            _logger = logger;
        }

        public void UpdateMetrics(IMetrics metrics)
        {
            for (var gen = 0; gen <= GC.MaxGeneration; gen++)
                metrics.Measure.Counter.Increment(dotnet_collection_count, GC.CollectionCount(gen), $"gen {gen}");

            metrics.Measure.Gauge.SetValue(dotnet_total_memory_bytes, GC.GetTotalMemory(false));
        }

        public void Dispose() { }
    }
}