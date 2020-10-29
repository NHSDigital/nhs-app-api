using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Created by DI")]
    internal sealed class AnonymousMetricLogger : IAnonymousMetricLogger
    {
        private readonly IMetricContext _metricContext;

        public AnonymousMetricLogger(IMetricContext metricContext) => _metricContext = metricContext;

        public Task AppointmentBookResult(AppointmentMetricData data) => LogMetric(data);

        public Task AppointmentCancelResult(AppointmentMetricData data) => LogMetric(data);

        private Task LogMetric(IMetricData data, [CallerMemberName] string action = "")
        {
            return MetricLogBuilder.Create(_metricContext, action).With(data).WriteMetricLog();
        }
    }
}