using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Created by DI")]
    internal sealed class MetricLogger : IMetricLogger
    {
        private readonly IMetricContext _metricContext;

        public MetricLogger(IMetricContext metricContext) => _metricContext = metricContext;

        public Task Login(LoginData data) => CreateMetricLog().With(data).WriteMetricLog();

        public Task UpliftStarted() => WriteMetricLog();

        public Task UserResearchOptIn() => WriteMetricLog();

        public Task UserResearchOptOut() => WriteMetricLog();

        public Task TermsAndConditionsInitialConsent() => WriteMetricLog();

        public Task MessageRead(MessageReadData data) => CreateMetricLog().With(data).WriteMetricLog();

        public Task NotificationsEnabled() => WriteMetricLog();

        public Task NotificationsDisabled() => WriteMetricLog();

        public Task NotificationsPrompt(NotificationsPromptData data) => CreateMetricLog().With(data).WriteMetricLog();

        public Task SilverIntegrationJumpOff(SilverIntegrationData data) => CreateMetricLog().With(data).WriteMetricLog();

        private Task WriteMetricLog([CallerMemberName] string action = "")
        {
            return CreateMetricLog(action).WriteMetricLog();
        }

        private MetricLogBuilder CreateMetricLog([CallerMemberName] string action = "")
        {
            return MetricLogBuilder.Create(_metricContext, action).With(IdentifyingMetricData());
        }

        private IEnumerable<KeyValuePair<string, string>> IdentifyingMetricData()
        {
            yield return new KeyValuePair<string, string>("NhsLoginId", _metricContext.NhsLoginId);
            yield return new KeyValuePair<string, string>("ProofLevel", _metricContext.ProofLevel.ToString());
        }
    }
}