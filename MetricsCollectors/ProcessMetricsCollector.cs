using App.Metrics;
using App.Metrics.Gauge;

using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class ProcessMetricsCollector : BaseMetricsCollector
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

        public ProcessMetricsCollector(IMetrics metrics, ILogger<ProcessMetricsCollector> logger) : base(metrics)
        {
            _logger = logger;
            _process = Process.GetCurrentProcess();
        }

        public override ValueTask UpdateAsync(CancellationToken stoppingToken)
        {
            if (!_setStartTime)
            {
                _process.Refresh();
                Metrics.Measure.Gauge.SetValue(process_start_time_milliseconds, new DateTimeOffset(_process.StartTime).ToUnixTimeMilliseconds());
                _setStartTime = true;
            }

            return default;
        }

        public override void Dispose()
        {
            _process.Dispose();
        }
    }
}