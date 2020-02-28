using App.Metrics;

using Aragas.QServer.Metrics;
using Aragas.QServer.Metrics.BackgroundServices;

using Microsoft.Extensions.DependencyInjection.Extensions;

using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPrometheusEndpoint(this IServiceCollection services, Func<IMetricsBuilder, IMetricsBuilder>? additional = null)
        {
            var metricsBuilder = new MetricsBuilder()
                .Configuration.Configure(options => options
                    .AddMachineNameTag()
                    .AddRuntimeTag()
                    .AddServerTag()
                    .AddGitTag())

                .OutputMetrics.AsPrometheusPlainText();
            metricsBuilder = additional?.Invoke(metricsBuilder) ?? metricsBuilder;
            services.AddMetrics(metricsBuilder);

            services.TryAddSingleton<ICpuUsageMonitor, CpuUsageMonitor>();
            services.AddHostedService(sp => (CpuUsageMonitor)sp.GetRequiredService<ICpuUsageMonitor>());

            services.AddMetricsReportingHostedService();

            return services;
        }

        public static IServiceCollection AddDefaultMetrics(this IServiceCollection services)
        {
            services.AddHostedService<StandardMetricsService>();

            return services;
        }
    }
}