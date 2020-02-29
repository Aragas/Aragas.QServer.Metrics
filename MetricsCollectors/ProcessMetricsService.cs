using App.Metrics;
using App.Metrics.Gauge;

using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;

namespace Aragas.QServer.Metrics
{
    public sealed class ProcessMetricsCollector : IMetricsCollector
    {
        private readonly GaugeOptions process_start_time_milliseconds = new GaugeOptions()
        {
            Name = "start_time_milliseconds",
            Context = "process",
            MeasurementUnit = Unit.Custom("Milliseconds")
        };
        private readonly GaugeOptions process_private_memory_bytes = new GaugeOptions()
        {
            Name = "private_memory_bytes",
            Context = "process",
            MeasurementUnit = Unit.Bytes
        };
        private readonly GaugeOptions process_working_set_bytes = new GaugeOptions()
        {
            Name = "working_set_bytes",
            Context = "process",
            MeasurementUnit = Unit.Bytes
        };

        private readonly ILogger _logger;
        private readonly Process _process;

        public ProcessMetricsCollector(ILogger<ProcessMetricsCollector> logger)
        {
            _logger = logger;
            _process = Process.GetCurrentProcess();
        }

        public void UpdateMetrics(IMetrics metrics)
        {
            _process.Refresh();

            metrics.Measure.Gauge.SetValue(process_start_time_milliseconds, new DateTimeOffset(_process.StartTime).ToUnixTimeMilliseconds());

            metrics.Measure.Gauge.SetValue(process_private_memory_bytes, _process.PrivateMemorySize64);
            metrics.Measure.Gauge.SetValue(process_working_set_bytes, _process.WorkingSet64);
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}