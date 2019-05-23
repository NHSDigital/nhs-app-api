using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsFetchConsentResultVisitor : ITermsAndConditionsFetchConsentResultVisitor<IActionResult>
    {
        public IActionResult Visit(TermsAndConditionsFetchConsentResult.Success result)
        {
            return new OkObjectResult(result);
        }
        
        public IActionResult Visit(TermsAndConditionsFetchConsentResult.NoConsentFound result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(TermsAndConditionsFetchConsentResult.FailureToFetchConsent result)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status463FailedToFetchConsent);
        }      
    }
}