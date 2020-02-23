using Microsoft.Extensions.Hosting;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Aragas.QServer.Metrics.BackgroundServices
{
    public class CpuUsageMonitor : BackgroundService, ICpuUsageMonitor
    {
        public double CpuUsagePercent { get; private set; }

        private readonly int _delay;

        public CpuUsageMonitor(int delay = 3000)
        {
            _delay = delay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var process = Process.GetCurrentProcess();
            while (!stoppingToken.IsCancellationRequested)
            {
                var startTime = DateTime.UtcNow;
                process.Refresh();
                var startCpuUsage = process.TotalProcessorTime;

                await Task.Delay(_delay);

                var endTime = DateTime.UtcNow;
                process.Refresh();
                var endCpuUsage = process.TotalProcessorTime;

                var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                var totalMsPassed = (endTime - startTime).TotalMilliseconds;

                var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

                CpuUsagePercent = cpuUsageTotal * 100;
            }
        }
    }
}