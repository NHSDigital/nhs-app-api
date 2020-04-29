using System;
using System.Globalization;
using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentAuditingVisitor : IUserSessionVisitor<Task<TermsAndConditionsRecordConsentResult>>
    {
        private readonly IAuditor _auditor;
        private readonly DateTimeOffset _dateOfConsent;
        private readonly ConsentRequest _consentRequest;
        private readonly Func<Task<TermsAndConditionsRecordConsentResult>> _action;

        public TermsAndConditionsRecordConsentAuditingVisitor(
            IAuditor auditor,
            ConsentRequest consentRequest,
            DateTimeOffset dateOfConsent,
            Func<Task<TermsAndConditionsRecordConsentResult>> action)
        {
            _auditor = auditor;
            _consentRequest = consentRequest;
            _dateOfConsent = dateOfConsent;
            _action = action;
        }

        public async Task<TermsAndConditionsRecordConsentResult> Visit(P5UserSession userSession)
            => await _action();

        public async Task<TermsAndConditionsRecordConsentResult> Visit(P9UserSession userSession)
        {
            return await _auditor
                .Audit()
                .Operation(AuditingOperations.TermsAndConditionsRecordConsentAuditType)
                .Details(
                    "Attempting to record patient consent - ConsentGiven={0}, AnalyticsCookieAccepted={1} at DateOfConsent={2}",
                    _consentRequest.ConsentGiven,
                    _consentRequest.AnalyticsCookieAccepted,
                    _dateOfConsent.ToString(CultureInfo.InvariantCulture))
                .Execute(_action);
        }
    }
}