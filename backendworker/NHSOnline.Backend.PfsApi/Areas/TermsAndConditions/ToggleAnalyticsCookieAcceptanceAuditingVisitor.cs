using System;
using System.Globalization;
using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class ToggleAnalyticsCookieAcceptanceAuditingVisitor : IUserSessionVisitor<Task<ToggleAnalyticsCookieAcceptanceResult>>
    {
        private readonly IAuditor _auditor;
        private readonly DateTimeOffset _dateOfConsent;
        private readonly bool _analyticsCookieAccepted;
        private readonly Func<Task<ToggleAnalyticsCookieAcceptanceResult>> _action;

        public ToggleAnalyticsCookieAcceptanceAuditingVisitor(
            IAuditor auditor,
            DateTimeOffset dateOfConsent,
            bool analyticsCookieAccepted,
            Func<Task<ToggleAnalyticsCookieAcceptanceResult>> action)
        {
            _auditor = auditor;
            _dateOfConsent = dateOfConsent;
            _analyticsCookieAccepted = analyticsCookieAccepted;
            _action = action;
        }

        public async Task<ToggleAnalyticsCookieAcceptanceResult> Visit(P5UserSession userSession)
            => await _action();

        public async Task<ToggleAnalyticsCookieAcceptanceResult> Visit(P9UserSession userSession)
        {
            return await _auditor
                .Audit()
                .Operation(AuditingOperations.TermsAndConditionsToggleAnalyticsCookieAcceptance)
                .Details(
                    "Attempting to toggle analytics cookie acceptance - AnalyticsCookieAccepted={0} at DateOfAnalyticsCookieToggle={1}",
                    _analyticsCookieAccepted,
                    _dateOfConsent.ToString(CultureInfo.InvariantCulture))
                .Execute(_action);
        }
    }
}