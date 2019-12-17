using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class ToggleAnalyticsCookieAcceptanceAuditingVisitor : IToggleAnalyticsCookieAcceptanceVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly DateTimeOffset _dateOfConsent;
        private readonly bool _analyticsCookieAccepted;
        private readonly ILogger<TermsAndConditionsController> _logger;

        public ToggleAnalyticsCookieAcceptanceAuditingVisitor(IAuditor auditor,
            ILogger<TermsAndConditionsController> logger, DateTimeOffset dateOfConsent,
            bool analyticsCookieAccepted)
        {
            _auditor = auditor;
            _dateOfConsent = dateOfConsent;
            _analyticsCookieAccepted = analyticsCookieAccepted;
            _logger = logger;
        }

        public async Task Visit(ToggleAnalyticsCookieAcceptanceResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditingOperations.TermsAndConditionsToggleAnalyticsCookieAcceptanceResponse,
                    "Analytics Cookie Consent toggled Successfully DateAnalyticsCookieAccepted: {0:O}" +
                    " analytics cookie acceptance - AnalyticsCookieAccepted: {1}",
                    _dateOfConsent, _analyticsCookieAccepted);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditingOperations.TermsAndConditionsToggleAnalyticsCookieAcceptanceResponse} {nameof(ToggleAnalyticsCookieAcceptanceResult.Success)}");
            }
        }

        public async Task Visit(ToggleAnalyticsCookieAcceptanceResult.Failure result)
        {
            try
            {
                await _auditor.Audit(AuditingOperations.TermsAndConditionsToggleAnalyticsCookieAcceptanceResponse,
                    "Failed to toggle analytics cookie DateAnalyticsCookieAccepted: {0:O}" +
                    " analytics cookie acceptance - AnalyticsCookieAccepted: {1}",
                    _dateOfConsent, _analyticsCookieAccepted);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditingOperations.TermsAndConditionsToggleAnalyticsCookieAcceptanceResponse} {nameof(ToggleAnalyticsCookieAcceptanceResult.Failure)}");
            }
        }
    }
}