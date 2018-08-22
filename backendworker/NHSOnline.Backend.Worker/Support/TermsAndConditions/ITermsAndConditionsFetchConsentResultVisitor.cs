namespace NHSOnline.Backend.Worker.Support.TermsAndConditions
{
    public interface ITermsAndConditionsFetchConsentResultVisitor<out T>
    {
        T Visit(TermsAndConditionsFetchConsentResult.Success success);
        T Visit(TermsAndConditionsFetchConsentResult.NoConsentFound noConsentFound);
        T Visit(TermsAndConditionsFetchConsentResult.FailureToFetchConsent failureToFetchConsent);       
    }
}