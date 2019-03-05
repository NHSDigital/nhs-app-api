using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentResultVisitor : ITermsAndConditionsRecordConsentResultVisitor<IActionResult>
    {
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.InitialConsentRecorded consentRecorded)
        {
            return new OkObjectResult(consentRecorded);
        }
        
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.UpdateConsentRecorded consentRecorded)
        {
            return new OkObjectResult(consentRecorded);
        }
        
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent failureToRecordConsent)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status462FailedToRecordConsent);
        }      
    }
}