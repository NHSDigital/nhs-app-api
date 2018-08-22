using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Support.TermsAndConditions;
using System;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentAuditingVisitor : ITermsAndConditionsRecordConsentResultVisitor<IActionResult>
    {
        private readonly IAuditor _auditor;
        private readonly bool _consentGiven;
        private readonly DateTimeOffset _dateOfConsent;
        private const string AuditType = Constants.AuditingTitles.TermsAndConditionsRecordConsentAuditTypeResponse;

        public TermsAndConditionsRecordConsentAuditingVisitor(IAuditor auditor, bool consentGiven, DateTimeOffset dateOfConsent)
        {
            _auditor = auditor;
            _consentGiven = consentGiven;
            _dateOfConsent = dateOfConsent;
        }
        
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.ConsentRecorded consentRecorded)
        {
            _auditor.Audit(AuditType, "Successfully recorded consent - ConsentGiven: {0} and DateOfConsent: {1:O}",
                _consentGiven, _dateOfConsent);
            return null;
        }

        public IActionResult Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent failureToRecordConsent)
        {
            _auditor.Audit(AuditType, "Failed to record consent - ConsentGiven: {0} and DateOfConsent: {1:O}",
                _consentGiven, _dateOfConsent);
            return null;
        }          
    }
}