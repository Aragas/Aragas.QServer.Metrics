using Aragas.QServer.Metrics;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Metrics.DotNetRuntime
{
    /// <summary>
    /// Configures what .NET core runtime metrics will be collected. 
    /// </summary>
    public class MetricsCollectorBuilder
    {
        public static MetricsCollectorBuilder Default()
        {
            return Customize()
                .WithCpuUsageMetrics()
                .WithDotNetMetrics()
                .WithFirstChanceExceptionMetrics()
                .WithProcessMetrics();
        }

        public static MetricsCollectorBuilder Customize()
        {
            return new MetricsCollectorBuilder();
        }


        private readonly List<Func<IServiceProvider, IMetricsCollector>> _metricsCollectors = new List<Func<IServiceProvider, IMetricsCollector>>();

        public MetricsCollectorBuilder WithCpuUsageMetrics()
        {
            _metricsCollectors.Add(ActivatorUtilities.GetServiceOrCreateInstance<CpuUsageMetricsCollector>);
            return this;
        }

        public MetricsCollectorBuilder WithDotNetMetrics()
        {
            _metricsCollectors.Add(ActivatorUtilities.GetServiceOrCreateInstance<DotNetMetricsCollector>);
            return this;
        }

        public MetricsCollectorBuilder WithFirstChanceExceptionMetrics()
        {
            _metricsCollectors.Add(ActivatorUtilities.GetServiceOrCreateInstance<FirstChanceExceptionMetricsCollector>);
            return this;
        }

        public MetricsCollectorBuilder WithProcessMetrics()
        {
            _metricsCollectors.Add(ActivatorUtilities.GetServiceOrCreateInstance<ProcessMetricsCollector>);
            return this;
        }

        public MetricsCollectorBuilder WithCustomCollector(IMetricsCollector metricsCollector)
        {
            _metricsCollectors.Add(_ => metricsCollector);
            return this;
        }

        public List<IMetricsCollector> Build(IServiceProvider serviceProvider) =>
            _metricsCollectors.Select(mc => mc(serviceProvider)).ToList();
    }
}