using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceDefinition
{
    public class ServiceDefinitionResultVisitor : IServiceDefinitionResultVisitor<IActionResult>
    {
        public IActionResult Visit(ServiceDefinitionResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(ServiceDefinitionResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(ServiceDefinitionResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(ServiceDefinitionResult.CustomError result, int errorCode)
        {
            return new StatusCodeResult(errorCode);
        }

        public IActionResult Visit(ServiceDefinitionResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(ServiceDefinitionResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(ServiceDefinitionResult.DemographicsBadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(ServiceDefinitionResult.DemographicsRetrievalFailed result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(ServiceDefinitionResult.DemographicsForbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(ServiceDefinitionResult.DemographicsInternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}