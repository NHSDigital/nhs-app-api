using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    [SuppressMessage("Microsoft.Performance","CA1812", Justification = "Created by DI")]
    internal sealed class MetricLogger : IMetricLogger
    {
        private readonly IMetricContext _metricContext;

        public MetricLogger(IMetricContext metricContext) => _metricContext = metricContext;

        public Task Login() => LogMetric();

        public Task UpliftStarted() => LogMetric();

        public Task UserResearchOptIn() => LogMetric();

        public Task UserResearchOptOut() => LogMetric();

        public Task TermsAndConditionsInitialConsent() => LogMetric();

        public Task MessageRead(MessageReadData data) => LogMetric(data);

        public Task NotificationsEnabled() => LogMetric();

        public Task NotificationsDisabled() => LogMetric();

        private Task LogMetric(IMetricData metricData = null, [CallerMemberName] string action = "")
        {
            var allMetricData = DefaultMetricData(action);

            if (metricData != null)
            {
                allMetricData = allMetricData.Concat(metricData.ToKeyValuePairs());
            }

            var metricLog = string.Join(" ", allMetricData.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            Console.Out.WriteLine(metricLog);

            return Task.CompletedTask;
        }

        private IEnumerable<KeyValuePair<string, string>> DefaultMetricData(string action)
        {
            var timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss:fff", CultureInfo.InvariantCulture);
            yield return new KeyValuePair<string, string>("Timestamp", timestamp);
            yield return new KeyValuePair<string, string>("NhsLoginId", _metricContext.NhsLoginId);
            yield return new KeyValuePair<string, string>("ProofLevel", _metricContext.ProofLevel.ToString());
            yield return new KeyValuePair<string, string>("OdsCode", _metricContext.OdsCode);
            yield return new KeyValuePair<string, string>("Action", action);
        }
    }
}