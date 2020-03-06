using App.Metrics;
using App.Metrics.Counter;

using Microsoft.Extensions.Logging;

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics.MetricsCollectors
{
    public sealed class FirstChanceExceptionMetricsCollector : BaseMetricsCollector
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

        public FirstChanceExceptionMetricsCollector(IMetrics metrics, ILogger<FirstChanceExceptionMetricsCollector> logger) : base(metrics)
        {
            _logger = logger;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            var tags = new MetricTags(new[] { "ExceptionType", "ExceptionMessage" }, new[] { e.Exception.GetType().FullName, e.Exception.Message });
            var exception = Metrics.Provider.Counter.Instance(dotnet_exception, tags);
            var exceptionTotal = Metrics.Provider.Counter.Instance(dotnet_exception_total);

            exception.Increment();
            exceptionTotal.Increment();
        }

        public override ValueTask UpdateAsync(CancellationToken stoppingToken) => default;

        public override void Dispose()
        {
            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
        }
    }
}