using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.TermsAndConditions;

namespace NHSOnline.Backend.Worker.Areas.TermsAndConditions
{
    public class TermsAndConditionsFetchConsentResultVisitor : ITermsAndConditionsFetchConsentResultVisitor<IActionResult>
    {
        public IActionResult Visit(TermsAndConditionsFetchConsentResult.Success success)
        {
            return new OkObjectResult(success);
        }
        
        public IActionResult Visit(TermsAndConditionsFetchConsentResult.NoConsentFound noConsentFound)
        {
            return new OkObjectResult(noConsentFound);
        }

        public IActionResult Visit(TermsAndConditionsFetchConsentResult.FailureToFetchConsent failureToFetchConsent)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status463FailedToFetchConsent);
        }      
    }
}