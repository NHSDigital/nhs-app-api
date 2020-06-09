using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class RepositoryUpdateAnalyticsCookieResultVisitor : IRepositoryUpdateResultVisitor<TermsAndConditionsRecord, ToggleAnalyticsCookieAcceptanceResult>
    {
        public ToggleAnalyticsCookieAcceptanceResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.NotFound result)
        {
            return new ToggleAnalyticsCookieAcceptanceResult.Failure();
        }

        public ToggleAnalyticsCookieAcceptanceResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.RepositoryError result)
        {
            return new ToggleAnalyticsCookieAcceptanceResult.Failure();
        }

        public ToggleAnalyticsCookieAcceptanceResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.NoChange result)
        {
            return new ToggleAnalyticsCookieAcceptanceResult.Success();
        }

        public ToggleAnalyticsCookieAcceptanceResult Visit(RepositoryUpdateResult<TermsAndConditionsRecord>.Updated result)
        {
            return new ToggleAnalyticsCookieAcceptanceResult.Success();
        }
    }
}