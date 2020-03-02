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

            services.AddMetricsReportingHostedService();

            return services;
        }

        public static IServiceCollection AddBuiltInMetrics(this IServiceCollection services, MetricsCollectorBuilder? builder = null)
        {
            services.TryAddSingleton<ICpuUsageMonitor, CpuUsageMonitor>();
            services.AddHostedService(sp => (CpuUsageMonitor) sp.GetRequiredService<ICpuUsageMonitor>());

            if (builder == null)
                builder = MetricsCollectorBuilder.Default();

            services.AddHostedService(sp => ActivatorUtilities.CreateInstance<MetricsCollectorService>(sp, builder.Build(sp)));

            return services;
        }

        public static IServiceCollection AddDotNetRuntimeMetrics(this IServiceCollection services)
        {
            services.AddHostedService(sp => ActivatorUtilities.CreateInstance<DotNetMetricsService>(sp));

            return services;
        }
    }
}