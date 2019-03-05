namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsRecordConsentResultVisitor<out T>
    {
        T Visit(TermsAndConditionsRecordConsentResult.InitialConsentRecorded consentRecorded);
        T Visit(TermsAndConditionsRecordConsentResult.UpdateConsentRecorded consentRecorded);
        T Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent failureToRecordConsent);       
    }
}