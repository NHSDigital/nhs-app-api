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

        public Task Login(LoginData data) => WriteMetricLog(data);

        public Task UpliftStarted(UpliftStartedData data) => WriteMetricLog(data);

        public Task UserResearchOptIn() => WriteMetricLog();

        public Task UserResearchOptOut() => WriteMetricLog();

        public Task TermsAndConditionsInitialConsent() => WriteMetricLog();

        public Task MessageRead(MessageReadData data) => WriteMetricLog(data);

        public Task NotificationsEnabled() => WriteMetricLog();

        public Task NotificationsDisabled() => WriteMetricLog();

        public Task NotificationsPrompt(NotificationsPromptData data) => WriteMetricLog(data);

        public Task OrganDonationWithdrawRegistration(OrganDonationData data) => WriteMetricLog(data);

        public Task OrganDonationGetRegistration(OrganDonationData data) => WriteMetricLog(data);

        public Task OrganDonationCreateRegistration(OrganDonationData data) => WriteMetricLog(data);

        public Task OrganDonationUpdateRegistration(OrganDonationData data) => WriteMetricLog(data);

        public Task SilverIntegrationJumpOff(SilverIntegrationData data) => WriteMetricLog(data);

        public Task MedicalRecordView(MedicalRecordData data) => WriteMetricLog(data);

        public Task NominatedPharmacyCreate(NominatedPharmacyData data) => WriteMetricLog(data);

        public Task NominatedPharmacyUpdate(NominatedPharmacyData data) => WriteMetricLog(data);

        public Task RepeatPrescriptionOrder(RepeatPrescriptionData data) => WriteMetricLog(data);

        private Task WriteMetricLog(IMetricData data, [CallerMemberName] string action = "")
        {
            return CreateMetricLog(action).With(data).WriteMetricLog();
        }

        private Task WriteMetricLog([CallerMemberName] string action = "")
        {
            return CreateMetricLog(action).WriteMetricLog();
        }

        private MetricLogBuilder CreateMetricLog(string action)
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