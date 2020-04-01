using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceDefinition
{
    public class ServiceDefinitionIsValidResultVisitor : IServiceDefinitionIsValidResultVisitor<IActionResult>
    {
        public IActionResult Visit(ServiceDefinitionIsValidResult.Valid result)
        {
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        public IActionResult Visit(ServiceDefinitionIsValidResult.Invalid result)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status580ServiceDefinitionUnavailable);
        }

        public IActionResult Visit(ServiceDefinitionIsValidResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(ServiceDefinitionIsValidResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}