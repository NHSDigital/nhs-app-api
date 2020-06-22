using System;
using System.Globalization;
using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentAuditingVisitor : IUserSessionVisitor<Task<TermsAndConditionsRecordConsentResult>>, ITermsAndConditionsFetchConsentResultVisitor<TermsAndConditionsRecordConsentResult>
    {
        private readonly IAuditor _auditor;
        private readonly DateTimeOffset _dateOfConsent;
        private readonly ConsentRequest _consentRequest;
        private readonly Func<Task<TermsAndConditionsRecordConsentResult>> _action;
        private bool? _previousConsentRequestFailed;
        private DateTimeOffset? _previousDateOfConsent;

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

        public Task<TermsAndConditionsRecordConsentResult> Visit(P5UserSession userSession) => AuditUserSession();

        public Task<TermsAndConditionsRecordConsentResult> Visit(P9UserSession userSession) => AuditUserSession();

        private async Task<TermsAndConditionsRecordConsentResult> AuditUserSession()
        {
            if (_previousConsentRequestFailed ?? false)
            {
                return new TermsAndConditionsRecordConsentResult.InternalServerError();
            }

            return await _auditor
                .Audit()
                .Operation(AuditingOperations.TermsAndConditionsRecordConsentAuditType)
                .Details(
                    "Attempting to record patient consent - ConsentGiven={0}, AnalyticsCookieAccepted={1}, previousDateOfConsent={2}, DateOfConsent={3}",
                    _consentRequest.ConsentGiven,
                    _consentRequest.AnalyticsCookieAccepted,
                    _previousDateOfConsent?.ToString(CultureInfo.InvariantCulture) ?? "null",
                    _dateOfConsent.ToString(CultureInfo.InvariantCulture))
                .Execute(_action);
        }

        public IUserSessionVisitor<Task<TermsAndConditionsRecordConsentResult>> Accept(TermsAndConditionsFetchConsentResult previousConsentResult)
        {
            previousConsentResult.Accept(this);
            return this;
        }

        public TermsAndConditionsRecordConsentResult Visit(TermsAndConditionsFetchConsentResult.Success result)
        {
            _previousDateOfConsent = result.Response.DateOfConsent;

            return new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();
        }

        public TermsAndConditionsRecordConsentResult Visit(TermsAndConditionsFetchConsentResult.NoConsentFound result)
        {
            _previousConsentRequestFailed = false;

            return new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();
        }

        public TermsAndConditionsRecordConsentResult Visit(TermsAndConditionsFetchConsentResult.InternalServerError result)
        {
            _previousConsentRequestFailed = true;

            return new TermsAndConditionsRecordConsentResult.InternalServerError();
        }
    }
}