namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsFetchConsentResultVisitor<out T>
    {
        T Visit(TermsAndConditionsFetchConsentResult.Success result);
        T Visit(TermsAndConditionsFetchConsentResult.NoConsentFound result);
        T Visit(TermsAndConditionsFetchConsentResult.FailureToFetchConsent result);       
    }
}