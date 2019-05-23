namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsRecordConsentResultVisitor<out T>
    {
        T Visit(TermsAndConditionsRecordConsentResult.InitialConsentRecorded result);
        T Visit(TermsAndConditionsRecordConsentResult.UpdateConsentRecorded result);
        T Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent result);       
    }
}