using System;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using Constants = NHSOnline.Backend.Support.Constants;
using ClinicalDecisionSupportConstants = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Constants;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceDefinition
{
    public class ServiceDefinitionController : Controller
    {
        private readonly IServiceDefinitionService _service;
        private readonly IFhirParameterHelpers _fhirParameterHelpers;
        private readonly ILogger<ServiceDefinitionController> _logger;

        public ServiceDefinitionController(
            IServiceDefinitionService service,
            IFhirParameterHelpers fhirParameterHelpers,
            ILogger<ServiceDefinitionController> logger)
        {
            _service = service;
            _fhirParameterHelpers = fhirParameterHelpers;
            _logger = logger;
        }

        [HttpPost]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}")]
        public async Task<IActionResult> GetServiceDefinition(
            [FromBody] ServiceDefinitionMetaData metaData,
            [FromRoute(Name = "provider")] string provider,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (ModelState.IsValid)
                {
                    var description = ClinicalDecisionSupportConstants.ServiceDefinitionDescriptions[metaData.Type];

                    _logger.LogInformation($"Starting online consultation for {description}. ODSCode: {userSession.CitizenIdUserSession.OdsCode ??= "None"}");

                    return (await _service.GetServiceDefinition(provider, metaData.Id, description, userSession))
                        .Accept(new ServiceDefinitionResultVisitor());
                }

                _logger.LogModelStateValidationFailure(ModelState);

                return new ServiceDefinitionResult.BadRequest()
                    .Accept(new ServiceDefinitionResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}/evaluate")]
        public async Task<IActionResult> EvaluateServiceDefinition(
            [FromRoute(Name = "provider")] string provider,
            [FromBody] Parameters parameters,
            [FromQuery] bool demographicsConsentGiven,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (parameters?.Parameter is null)
                {
                    _logger.LogError("Parameters cannot be null");

                    return new ServiceDefinitionResult.BadRequest().Accept(new ServiceDefinitionResultVisitor());
                }

                parameters = _fhirParameterHelpers.RemoveServiceDefinitionMetadataFromParameters(parameters, out var metaData);

                if (!(metaData is null))
                {
                    var description = ClinicalDecisionSupportConstants.ServiceDefinitionDescriptions[metaData.Type];

                    _logger.LogInformation($"Evaluating for {description}. ODSCode: {userSession.CitizenIdUserSession.OdsCode ??= "None"}");

                    return (await _service.EvaluateServiceDefinition(
                            provider,
                            metaData.Id,
                            description,
                            parameters,
                            "true".Equals(Request.Headers[Constants.HttpHeaders.JavascriptDisabled],
                                StringComparison.Ordinal),
                            demographicsConsentGiven,
                            userSession))
                        .Accept(new ServiceDefinitionResultVisitor());
                }

                _logger.LogInformation($"{nameof(_fhirParameterHelpers.RemoveServiceDefinitionMetadataFromParameters)} returned null metaData");

                return new ServiceDefinitionResult.BadRequest().Accept(new ServiceDefinitionResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}/details")]
        public IActionResult GetProviderDetails([FromRoute(Name = "provider")] string provider)
        {
            try
            {
                _logger.LogEnter();

                return _service.GetProviderName(provider).Accept(new ServiceDefinitionResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}/isValid")]
        public async Task<IActionResult> GetServiceDefinitionIsValid(
            [FromRoute(Name = "provider")] string provider,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                return (await _service.GetServiceDefinitionIsValid(provider, userSession))
                    .Accept(new ServiceDefinitionIsValidResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<IActionResult> GetServiceDefinition(string provider, string serviceDefinitionId, ServiceDefinitionType type, P9UserSession userSession)
        {
            var description = ClinicalDecisionSupportConstants.ServiceDefinitionDescriptions[type];

            _logger.LogInformation($"Starting online consultation for {description}. " +
                                   $"ODSCode: {userSession.CitizenIdUserSession.OdsCode ??= "None"}");

            return (await _service.GetServiceDefinition(provider, serviceDefinitionId, description, userSession))
                .Accept(new ServiceDefinitionResultVisitor());
        }

        private async Task<IActionResult> EvaluateServiceDefinition(
            string provider,
            string serviceDefinitionId,
            ServiceDefinitionType type,
            P9UserSession userSession,
            Parameters parameters,
            bool demographicsConsentGiven)
        {
            var description = ClinicalDecisionSupportConstants.ServiceDefinitionDescriptions[type];

            _logger.LogInformation($"Evaluating for {description}. " +
                                   $"ODSCode: {userSession.CitizenIdUserSession.OdsCode ??= "None"}");

            return (await _service.EvaluateServiceDefinition(
                    provider,
                    serviceDefinitionId,
                    description,
                    parameters,
                    "true".Equals(Request.Headers[Constants.HttpHeaders.JavascriptDisabled],
                        StringComparison.Ordinal),
                    demographicsConsentGiven,
                    userSession))
                .Accept(new ServiceDefinitionResultVisitor());
        }
    }
}