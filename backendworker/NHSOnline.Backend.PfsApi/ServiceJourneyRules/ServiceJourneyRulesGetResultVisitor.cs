using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    internal class ServiceJourneyRulesGetResultVisitor : IServiceJourneyRulesConfigResultVisitor<IActionResult>
    {
        public IActionResult Visit(ServiceJourneyRulesConfigResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(ServiceJourneyRulesConfigResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(ServiceJourneyRulesConfigResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
