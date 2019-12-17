using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class ToggleAnalyticsCookieAcceptanceResultVisitor : IToggleAnalyticsCookieAcceptanceVisitor<IActionResult>
    {
        public IActionResult Visit(ToggleAnalyticsCookieAcceptanceResult.Success result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(ToggleAnalyticsCookieAcceptanceResult.Failure result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}