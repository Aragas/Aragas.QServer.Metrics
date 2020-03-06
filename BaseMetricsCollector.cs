using App.Metrics;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics
{
    public abstract class BaseMetricsCollector : IDisposable, IAsyncDisposable
    {
        protected IMetrics Metrics { get; }
        protected BaseMetricsCollector(IMetrics metrics) { Metrics = metrics; }

        public abstract ValueTask UpdateAsync(CancellationToken stoppingToken);

        public abstract void Dispose();
        public virtual ValueTask DisposeAsync()
        {
            try
            {
                Dispose();
                return default;
            }
            catch (Exception exception)
            {
                return new ValueTask(Task.FromException(exception));
            }
        }
    }
}