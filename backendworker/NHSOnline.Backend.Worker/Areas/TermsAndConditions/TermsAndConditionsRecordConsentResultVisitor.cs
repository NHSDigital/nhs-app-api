using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Support.TermsAndConditions;

namespace NHSOnline.Backend.Worker.Areas.TermsAndConditions
{
    public class TermsAndConditionsRecordConsentResultVisitor : ITermsAndConditionsRecordConsentResultVisitor<IActionResult>
    {
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.ConsentRecorded consentRecorded)
        {
            return new OkObjectResult(consentRecorded);
        }

        public IActionResult Visit(TermsAndConditionsRecordConsentResult.FailureToRecordConsent failureToRecordConsent)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status462FailedToRecordConsent);
        }      
    }
}