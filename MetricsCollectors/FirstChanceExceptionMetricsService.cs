using App.Metrics;
using App.Metrics.Counter;

using Microsoft.Extensions.Logging;

using System;
using System.Runtime.ExceptionServices;

namespace Aragas.QServer.Metrics
{
    public sealed class FirstChanceExceptionMetricsCollector : IMetricsCollector
    {
        private readonly CounterOptions dotnet_exception_count = new CounterOptions()
        {
            Name = "exception_count",
            Context = "application",
            MeasurementUnit = Unit.Errors
        };

        private readonly ILogger _logger;
        private IMetrics _metrics;

        public FirstChanceExceptionMetricsCollector(ILogger<FirstChanceExceptionMetricsCollector> logger)
        {
            _logger = logger;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        }

        public void UpdateMetrics(IMetrics metrics)
        {
            _metrics = metrics;
        }
        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            _metrics.Measure.Counter.Increment(dotnet_exception_count);
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
        }
    }
}