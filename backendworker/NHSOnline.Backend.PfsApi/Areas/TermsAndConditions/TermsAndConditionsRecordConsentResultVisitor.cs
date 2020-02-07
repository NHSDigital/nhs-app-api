using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class
        TermsAndConditionsRecordConsentResultVisitor : ITermsAndConditionsRecordConsentResultVisitor<IActionResult>
    {
        public IActionResult Visit(TermsAndConditionsRecordConsentResult.InitialConsentRecorded result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(TermsAndConditionsRecordConsentResult.UpdateConsentRecorded result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(TermsAndConditionsRecordConsentResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}