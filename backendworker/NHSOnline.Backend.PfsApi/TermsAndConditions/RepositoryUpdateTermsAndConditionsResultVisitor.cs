using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class RepositoryUpdateTermsAndConditionsResultVisitor : IRepositoryUpdateResultVisitor<TermsAndConditionsRecord, TermsAndConditionsRecordConsentResult>
    {
        public TermsAndConditionsRecordConsentResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.NotFound result)
        {
            return new TermsAndConditionsRecordConsentResult.InternalServerError();
        }

        public TermsAndConditionsRecordConsentResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.RepositoryError result)
        {
            return new TermsAndConditionsRecordConsentResult.InternalServerError();
        }

        public TermsAndConditionsRecordConsentResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.NoChange result)
        {
            return new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();
        }

        public TermsAndConditionsRecordConsentResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.Updated result)
        {
            return new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();
        }
    }
}