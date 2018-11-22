using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.TermsAndConditions;

namespace NHSOnline.Backend.Worker.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentAuditingVisitor : ITermsAndConditionsRecordConsentResultVisitor<IActionResult>
    {
        private readonly IAuditor _auditor;
        private readonly bool _consentGiven;
        private readonly DateTimeOffset _dateOfConsent;
        private readonly bool _analyticsCookieAccepted;
        
        private const string TermsAndConditionsAuditType = Constants.AuditingTitles.TermsAndConditionsRecordConsentAuditTypeResponse;
        private const string AnalyticsCookieAuditType = Constants.AuditingTitles.TermsAndConditionsAnalyticsCookieAcceptance;
        
        public TermsAndConditionsRecordConsentAuditingVisitor(IAuditor auditor, bool consentGiven, DateTimeOffset dateOfConsent,
            bool analyticsCookieAccepted)
        {
            _auditor = auditor;
            _consentGiven = consentGiven;
            _dateOfConsent = dateOfConsent;
            _analyticsCookieAccepted = analyticsCookieAccepted;
        }
        
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.ConsentRecorded consentRecorded)
        {
            _auditor.Audit(TermsAndConditionsAuditType, "Successfully recorded consent - ConsentGiven: {0} and DateOfConsent: {1:O}",
                _consentGiven, _dateOfConsent);
            
            _auditor.Audit(AnalyticsCookieAuditType, "Sucessfully recorded analytics cookie acceptance - AnalyticsCookieAccepted: {0}{1}",
                _analyticsCookieAccepted,
                _analyticsCookieAccepted
                    ? string.Format(CultureInfo.InvariantCulture, " and DateAnalyticsCookieAccepted: {0:O}", _dateOfConsent)
                    : "");
            
            return null;
        }

        public IActionResult Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent failureToRecordConsent)
        {
            _auditor.Audit(TermsAndConditionsAuditType, "Failed to record consent - ConsentGiven: {0} and DateOfConsent: {1:O}",
                _consentGiven, _dateOfConsent);
            
            _auditor.Audit(AnalyticsCookieAuditType, "Failed to record analytics cookie acceptance - AnalyticsCookieAccepted: {0}{1}",
                _analyticsCookieAccepted,
                _analyticsCookieAccepted
                    ? string.Format(CultureInfo.InvariantCulture,  " and DateAnalyticsCookieAccepted: {0:O}", _dateOfConsent)
                    : "");
            
            return null;
        }          
    }
}