using App.Metrics;

using System;

namespace Aragas.QServer.Metrics
{
    public interface IMetricsCollector : IDisposable
    {
        public void UpdateMetrics(IMetrics metrics);
    }
}