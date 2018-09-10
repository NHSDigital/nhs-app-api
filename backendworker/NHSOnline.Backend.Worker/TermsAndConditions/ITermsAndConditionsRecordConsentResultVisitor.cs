namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    public interface ITermsAndConditionsRecordConsentResultVisitor<out T>
    {
        T Visit(TermsAndConditionsRecordConsentResult.ConsentRecorded consentRecorded);
        T Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent failureToRecordConsent);       
    }
}