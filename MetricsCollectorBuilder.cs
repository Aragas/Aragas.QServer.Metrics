using Aragas.QServer.Metrics.MetricsCollectors;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Aragas.QServer.Metrics
{
    /// <summary>
    /// Configures what .NET core runtime metrics will be collected. 
    /// </summary>
    public class MetricsCollectorBuilder
    {
        public static MetricsCollectorBuilder Default()
        {
            return Customize()
                //.WithCpuUsageMetrics()
                //.WithDotNetMetrics()
                .WithFirstChanceExceptionMetrics()
                //.WithProcessMetrics()
                ;
        }

        public static MetricsCollectorBuilder Customize()
        {
            return new MetricsCollectorBuilder();
        }


        private readonly List<Func<IServiceProvider, BaseMetricsCollector>> _metricsCollectors = new List<Func<IServiceProvider, BaseMetricsCollector>>();

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

        public MetricsCollectorBuilder WithCustomCollector(BaseMetricsCollector metricsCollector)
        {
            _metricsCollectors.Add(_ => metricsCollector);
            return this;
        }

        public List<BaseMetricsCollector> Build(IServiceProvider serviceProvider) => _metricsCollectors.Select(mc => mc(serviceProvider)).ToList();
    }
}