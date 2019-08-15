using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceDefinition
{
    public class ServiceDefinitionListResultVisitor : IServiceDefinitionListResultVisitor<IActionResult>
    {
        public IActionResult Visit(ServiceDefinitionListResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(ServiceDefinitionListResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(ServiceDefinitionListResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(ServiceDefinitionListResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(ServiceDefinitionListResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }
    }
}