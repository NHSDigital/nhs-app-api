using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Demographics;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    internal class DemographicsResultVisitor : IDemographicsResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetDemographicsResult.UserHasNoAccess result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(GetDemographicsResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetDemographicsResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetDemographicsResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}