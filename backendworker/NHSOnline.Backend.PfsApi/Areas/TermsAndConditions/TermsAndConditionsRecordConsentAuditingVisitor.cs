using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentAuditingVisitor : ITermsAndConditionsRecordConsentResultVisitor<IActionResult>
    {
        private readonly IAuditor _auditor;
        private readonly bool _consentGiven;
        private readonly DateTimeOffset _dateOfConsent;
        private readonly bool _analyticsCookieAccepted;
        private readonly bool _isUpdatingConsent;
                
        public TermsAndConditionsRecordConsentAuditingVisitor(IAuditor auditor, bool consentGiven, DateTimeOffset dateOfConsent,
            bool analyticsCookieAccepted, bool isUpdatingConsent)
        {
            _auditor = auditor;
            _consentGiven = consentGiven;
            _dateOfConsent = dateOfConsent;
            _analyticsCookieAccepted = analyticsCookieAccepted;
            _isUpdatingConsent = isUpdatingConsent;
        }
        
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.InitialConsentRecorded result)
        {
            AuditConsent("Initial Consent Successfully recorded" );
            return null;
        }
        
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.UpdateConsentRecorded result)
        {
            AuditConsent("Updated Consent Successfully recorded" );
            return null;
        }
        
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent result)
        { 
            AuditConsent("Failed to record" );          
            return null;
        }


        private void AuditConsent(string message)
        {
            _auditor.Audit(AuditingOperations.TermsAndConditionsRecordConsentAuditTypeResponse, message + " consent - ConsentGiven: {0} and DateOfConsent: {1:O}",
                _consentGiven, _dateOfConsent);

            if (!_isUpdatingConsent)
            {
                _auditor.Audit(AuditingOperations.TermsAndConditionsAnalyticsCookieAcceptance,
                    message + " analytics cookie acceptance - AnalyticsCookieAccepted: {0}{1}",
                    _analyticsCookieAccepted,
                    _analyticsCookieAccepted
                        ? string.Format(CultureInfo.InvariantCulture, " and DateAnalyticsCookieAccepted: {0:O}",
                            _dateOfConsent)
                        : "");
            }
        }
    }
}