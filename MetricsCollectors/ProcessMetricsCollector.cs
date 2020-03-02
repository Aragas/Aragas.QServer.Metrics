using App.Metrics;
using App.Metrics.Gauge;

using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class ProcessMetricsCollector : IMetricsCollector
    {
        private readonly GaugeOptions process_start_time_milliseconds = new GaugeOptions()
        {
            Name = "start_time_milliseconds",
            Context = "process",
            MeasurementUnit = Unit.Custom("Milliseconds")
        };

        private readonly ILogger _logger;
        private readonly Process _process;
        private bool _setStartTime;

        public ProcessMetricsCollector(ILogger<ProcessMetricsCollector> logger)
        {
            _logger = logger;
            _process = Process.GetCurrentProcess();
        }

        public void UpdateMetrics(IMetrics metrics)
        {
            if (!_setStartTime)
            {
                _process.Refresh();
                metrics.Measure.Gauge.SetValue(process_start_time_milliseconds, new DateTimeOffset(_process.StartTime).ToUnixTimeMilliseconds());
                _setStartTime = true;
            }
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}