namespace Aragas.QServer.Metrics.BackgroundServices
{
    public interface ICpuUsageMonitor
    {
        double CpuUsagePercent { get; }
    }
}