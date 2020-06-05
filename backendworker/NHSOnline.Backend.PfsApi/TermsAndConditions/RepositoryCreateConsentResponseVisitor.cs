using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class RepositoryCreateConsentResponseVisitor : IRepositoryCreateResultVisitor<TermsAndConditionsRecord, TermsAndConditionsRecordConsentResult>
    {
        public TermsAndConditionsRecordConsentResult Visit(RepositoryCreateResult<TermsAndConditionsRecord>.Created result)
        {
            return new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();
        }

        public TermsAndConditionsRecordConsentResult Visit(RepositoryCreateResult<TermsAndConditionsRecord>.InternalServerError result)
        {
            return new TermsAndConditionsRecordConsentResult.InternalServerError();
        }

        public TermsAndConditionsRecordConsentResult Visit(RepositoryCreateResult<TermsAndConditionsRecord>.RepositoryError result)
        {
            return new TermsAndConditionsRecordConsentResult.InternalServerError();
        }
    }
}