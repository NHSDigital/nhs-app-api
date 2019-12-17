namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface IToggleAnalyticsCookieAcceptanceVisitor<out T>
    {
        T Visit(ToggleAnalyticsCookieAcceptanceResult.Success result);
        T Visit(ToggleAnalyticsCookieAcceptanceResult.Failure result);
    }
}