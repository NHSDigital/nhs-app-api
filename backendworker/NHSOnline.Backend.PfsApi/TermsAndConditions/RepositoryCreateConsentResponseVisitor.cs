using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class RepositoryCreateConsentResponseVisitor : IRepositoryCreateResultVisitor<TermsAndConditionsRecord, TermsAndConditionsRecordConsentResult>
    {
        private readonly IMetricLogger _metricLogger;

        public RepositoryCreateConsentResponseVisitor(IMetricLogger metricLogger)
        {
            _metricLogger = metricLogger;
        }
        public TermsAndConditionsRecordConsentResult Visit(RepositoryCreateResult<TermsAndConditionsRecord>.Created result)
        {
            _metricLogger.TermsAndConditionsInitialConsent();
            return new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();
        }

        public TermsAndConditionsRecordConsentResult Visit(RepositoryCreateResult<TermsAndConditionsRecord>.RepositoryError result)
        {
            return new TermsAndConditionsRecordConsentResult.InternalServerError();
        }
    }
}