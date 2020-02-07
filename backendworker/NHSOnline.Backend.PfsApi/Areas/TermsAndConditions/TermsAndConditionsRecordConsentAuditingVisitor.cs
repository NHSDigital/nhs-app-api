using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentAuditingVisitor : ITermsAndConditionsRecordConsentResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<TermsAndConditionsController> _logger;
        private readonly DateTimeOffset _dateOfConsent;
        private readonly ConsentRequest _consentRequest;

        private const string AuditType = AuditingOperations.TermsAndConditionsRecordConsentAuditTypeResponse;

        public TermsAndConditionsRecordConsentAuditingVisitor(
            IAuditor auditor,
            ILogger<TermsAndConditionsController> logger,
            ConsentRequest consentRequest,
            DateTimeOffset dateOfConsent)
        {
            _auditor = auditor;
            _logger = logger;
            _consentRequest = consentRequest;
            _dateOfConsent = dateOfConsent;
        }

        public async Task Visit(TermsAndConditionsRecordConsentResult.InitialConsentRecorded result)
        {
            try
            {
                await AuditConsent("Initial Consent Successfully recorded" );
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType}, " +
                                    $"{nameof(TermsAndConditionsRecordConsentResult.InitialConsentRecorded)}");
            }
        }

        public async Task Visit(TermsAndConditionsRecordConsentResult.UpdateConsentRecorded result)
        {
            try
            {
                await AuditConsent("Updated Consent Successfully recorded" );
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType}, " +
                                    $"{nameof(TermsAndConditionsRecordConsentResult.UpdateConsentRecorded)}");
            }
        }

        public async Task Visit(TermsAndConditionsRecordConsentResult.InternalServerError result)
        {
            try
            {
                await AuditConsent("Failed to record" );
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType}, " +
                                    $"{nameof(TermsAndConditionsRecordConsentResult.InternalServerError)}");
            }
        }

        private async Task AuditConsent(string message)
        {
            await _auditor.Audit(AuditingOperations.TermsAndConditionsRecordConsentAuditTypeResponse,
                message + " consent - ConsentGiven={0},  AnalyticsCookieAcceptance={1} at DateOfConsent={2:O}",
                _consentRequest.ConsentGiven, _consentRequest.AnalyticsCookieAccepted, _dateOfConsent);
        }
    }
}