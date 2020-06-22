using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsFetchPreviousConsentResultVisitor:
        ITermsAndConditionsFetchPreviousConsentResultVisitor<IActionResult>
    {
        public IActionResult Visit(TermsAndConditionsFetchPreviousConsentResult.Success result)
        {
            return new OkObjectResult(result.Response.DateOfConsent);
        }

        public IActionResult Visit(TermsAndConditionsFetchPreviousConsentResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}