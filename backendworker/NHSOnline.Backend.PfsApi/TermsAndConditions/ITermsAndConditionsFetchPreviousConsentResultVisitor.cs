namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsFetchPreviousConsentResultVisitor<out T>
    {
        T Visit(TermsAndConditionsFetchPreviousConsentResult.Success result);
        T Visit(TermsAndConditionsFetchPreviousConsentResult.InternalServerError result);
    }
}