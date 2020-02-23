using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Histogram;

using Aragas.QServer.Metrics.BackgroundServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics
{
    public class StandardMetricsService : BackgroundService
    {
        private readonly GaugeOptions process_start_time_milliseconds = new GaugeOptions()
        {
            Name = "Process Start Time Milliseconds",
            MeasurementUnit = Unit.Custom("Milliseconds")
        };
        private readonly HistogramOptions process_cpu_usage_percent = new HistogramOptions()
        {
            Name = "Process CPU Usage Percent",
            MeasurementUnit = Unit.Percent
        };
        private readonly GaugeOptions process_private_memory_bytes = new GaugeOptions()
        {
            Name = "Process Private Memory Bytes",
            MeasurementUnit = Unit.Bytes
        };
        private readonly GaugeOptions process_working_set_bytes = new GaugeOptions()
        {
            Name = "Process Working Set Bytes",
            MeasurementUnit = Unit.Bytes
        };
        private readonly GaugeOptions dotnet_total_memory_bytes = new GaugeOptions()
        {
            Name = "dotnet Total Memory Bytes",
            MeasurementUnit = Unit.Bytes
        };
        private readonly CounterOptions dotnet_collection_count = new CounterOptions()
        {
            Name = "dotnet Collection Count",
            MeasurementUnit = Unit.Items
        };
        private readonly CounterOptions dotnet_exception_count = new CounterOptions()
        {
            Name = "dotnet Exception Count",
            MeasurementUnit = Unit.Errors
        };

        private readonly IMetrics _metrics;
        private readonly ICpuUsageMonitor _cpuUsageMonitor;
        private readonly ILogger _logger;
        private readonly int _delay;
        private readonly Process _process;

        public StandardMetricsService(IMetrics metrics, ICpuUsageMonitor cpuUsageMonitor, ILogger<StandardMetricsService> logger, int delay = 3000)
        {
            _metrics = metrics;
            _cpuUsageMonitor = cpuUsageMonitor;
            _logger = logger;
            _delay = delay;
            _process = Process.GetCurrentProcess();

            _metrics.Measure.Gauge.SetValue(process_start_time_milliseconds, new DateTimeOffset(_process.StartTime).ToUnixTimeMilliseconds());

            _metrics.Measure.Counter.Increment(dotnet_exception_count);
            _metrics.Measure.Counter.Decrement(dotnet_exception_count);
            AppDomain.CurrentDomain.FirstChanceException += (s, e) => _metrics.Measure.Counter.Increment(dotnet_exception_count);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{TypeName}: Starting reporting. Delay:{Delay}", GetType().Name, _delay);

            while (!stoppingToken.IsCancellationRequested)
            {
                _process.Refresh();

                for (var gen = 0; gen <= GC.MaxGeneration; gen++)
                    _metrics.Measure.Counter.Increment(dotnet_collection_count, GC.CollectionCount(gen), $"gen {gen}");

                _metrics.Measure.Gauge.SetValue(dotnet_total_memory_bytes, GC.GetTotalMemory(false));
                _metrics.Measure.Gauge.SetValue(process_private_memory_bytes, _process.PrivateMemorySize64);
                _metrics.Measure.Gauge.SetValue(process_working_set_bytes, _process.WorkingSet64);
                _metrics.Measure.Histogram.Update(process_cpu_usage_percent, (long) _cpuUsageMonitor.CpuUsagePercent * 100 * 10);

                await Task.Delay(_delay, stoppingToken);
            }
        }
    }
}