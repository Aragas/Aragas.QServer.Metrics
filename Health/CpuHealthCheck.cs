using App.Metrics.Health;

using Aragas.QServer.Metrics.BackgroundServices;

using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Health
{
    public sealed class CpuHealthCheck : HealthCheck
    {
        private readonly ICpuUsageMonitor _cpuUsageMonitor;

        public CpuHealthCheck(ICpuUsageMonitor cpuUsageMonitor) : base(nameof(CpuHealthCheck))
        {
            _cpuUsageMonitor = cpuUsageMonitor;
        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default)
        {
            var usage = _cpuUsageMonitor.CpuUsagePercent;
            var message = $"CPU Usage {usage}%";
            if (usage > 90)
                return new ValueTask<HealthCheckResult>(HealthCheckResult.Unhealthy(message));
            if (usage > 80)
                return new ValueTask<HealthCheckResult>(HealthCheckResult.Degraded(message));

            return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy(message));
        }
    }
}