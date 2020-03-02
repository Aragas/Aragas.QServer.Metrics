using App.Metrics;
using App.Metrics.Counter;

using Microsoft.Extensions.Logging;

using System;
using System.Runtime.ExceptionServices;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class FirstChanceExceptionMetricsCollector : IMetricsCollector
    {
        private readonly CounterOptions dotnet_exception_total = new CounterOptions()
        {
            Name = "exception_total",
            Context = "application",
            MeasurementUnit = Unit.Errors
        };
        private readonly CounterOptions dotnet_exception = new CounterOptions()
        {
            Name = "exception",
            Context = "application",
            MeasurementUnit = Unit.Errors
        };

        private readonly ILogger _logger;
        private static readonly object _lock = new object();
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

        private long _counter = 0;
        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (_metrics == null)
            {
                lock (_lock)
                {
                    _counter++;
                }
            }
            else
            {
                var tags = new MetricTags(
                    new[] { "ExceptionType", "ExceptionMessage" },
                    new[] { e.Exception.GetType().FullName, e.Exception.Message });
                var exception = _metrics.Provider.Counter.Instance(dotnet_exception, tags);
                var exceptionTotal = _metrics.Provider.Counter.Instance(dotnet_exception_total);

                lock (_lock)
                {
                    if (_counter > 0)
                    {
                        exception.Increment(_counter);
                        exceptionTotal.Increment(_counter);
                        _counter = 0;
                    }
                }

                exception.Increment();
                exceptionTotal.Increment();
            }
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
        }
    }
}