using System;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using Constants = NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceDefinition
{
    public class ServiceDefinitionController : Controller
    {
        private readonly IServiceDefinitionService _service;
        private readonly ILogger<ServiceDefinitionController> _logger;

        public ServiceDefinitionController(
            IServiceDefinitionService service,
            ILoggerFactory loggerFactory)
        {
            _service = service;
            _logger = loggerFactory.CreateLogger<ServiceDefinitionController>();
        }

        [HttpGet]
        [Route("fhir/ServiceDefinition/{provider}/{id}")]
        [ApiVersionRoute("service-definition/{provider}/{id}")]
        public async Task<IActionResult> GetServiceDefinitionsById([FromRoute(Name = "id")] string serviceDefinitionId, 
          [FromRoute(Name = "provider")] string provider)
        {
            try
            {
                _logger.LogEnter();

                var visitor = new ServiceDefinitionResultVisitor();

                var userSession = HttpContext.GetUserSession();

                _logger.LogInformation($"Starting consultation with ServiceDefinition: {serviceDefinitionId}. ODSCode: {userSession.GpUserSession.OdsCode}");

                var result = await _service.GetServiceDefinitionById(provider, serviceDefinitionId, userSession);

                return result.Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [Route("fhir/ServiceDefinition/{provider}/{id}/$evaluate")]
        [ApiVersionRoute("service-definition/{provider}/{id}/$evaluate")]
        public async Task<IActionResult> EvaluateServiceDefinition(
            [FromRoute(Name = "provider")] string provider,
            [FromRoute(Name = "id")] string serviceDefinitionId,
            [FromBody] Parameters parameters,
            [FromQuery] bool demographicsConsentGiven
        )
        {
            try
            {
                var visitor = new ServiceDefinitionResultVisitor();

                _logger.LogEnter();

                if (parameters == null)
                {
                    _logger.LogError("Parameters cannot be null");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }
                
                var userSession = HttpContext.GetUserSession();
            
                _logger.LogInformation($"Evaluating ServiceDefinition: {serviceDefinitionId}. ODSCode: {userSession.GpUserSession.OdsCode}");

                return (await _service.EvaluateServiceDefinition(
                        provider,
                        serviceDefinitionId,
                        parameters,
                        "true".Equals(Request.Headers[Constants.HttpHeaders.JavascriptDisabled],
                            StringComparison.Ordinal),
                        demographicsConsentGiven,
                        userSession))
                    .Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Route("fhir/ServiceDefinition/providerName/{provider}")]
        [ApiVersionRoute("service-definition/provider-name/{provider}")]
        public IActionResult GetProviderName([FromRoute(Name = "provider")] string provider)
        {
            _logger.LogEnter();
            try
            {
                var visitor = new ServiceDefinitionResultVisitor();
                var result = _service.GetProviderName(provider);

                return result.Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}