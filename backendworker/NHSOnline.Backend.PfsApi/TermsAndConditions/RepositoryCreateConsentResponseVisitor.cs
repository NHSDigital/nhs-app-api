using System.Threading.Tasks;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class RepositoryCreateConsentResponseVisitor : IRepositoryCreateResultVisitor<TermsAndConditionsRecord, Task<TermsAndConditionsRecordConsentResult>>
    {
        private readonly IMetricLogger<UserSessionMetricContext> _metricLogger;

        public RepositoryCreateConsentResponseVisitor(IMetricLogger<UserSessionMetricContext> metricLogger)
        {
            _metricLogger = metricLogger;
        }
        public async Task<TermsAndConditionsRecordConsentResult> Visit(RepositoryCreateResult<TermsAndConditionsRecord>.Created result)
        {
            await _metricLogger.TermsAndConditionsInitialConsent();
            return new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();
        }

        public async Task<TermsAndConditionsRecordConsentResult> Visit(RepositoryCreateResult<TermsAndConditionsRecord>.RepositoryError result)
        {
            return await Task.FromResult(new TermsAndConditionsRecordConsentResult.InternalServerError());
        }
    }
}