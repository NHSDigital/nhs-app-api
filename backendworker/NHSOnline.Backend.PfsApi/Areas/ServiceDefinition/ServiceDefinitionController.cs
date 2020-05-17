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

        // When removing v1 endpoints, ensure to remove the associated CensorFilter from appSettings.json
        [HttpGet]
        [ApiVersion("1"), ApiVersionRoute("service-definition/{provider}/{id}")]
        public async Task<IActionResult> GetServiceDefinitionByIdV1(
            [FromRoute(Name = "id")] string serviceDefinitionId,
            [FromRoute(Name = "provider")] string provider,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                return await GetServiceDefinitionById(provider, serviceDefinitionId, ServiceDefinitionType.Unknown, userSession);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}")]
        public async Task<IActionResult> GetServiceDefinitionByIdV2(
            [FromBody] ServiceDefinitionMetaData metaData,
            [FromRoute(Name = "provider")] string provider,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (ModelState.IsValid)
                {
                    return await GetServiceDefinitionById(provider, metaData.Id, metaData.Type, userSession);
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
        [ApiVersion("1"), ApiVersionRoute("service-definition/{provider}/{id}/$evaluate")]
        public async Task<IActionResult> EvaluateServiceDefinitionV1(
            [FromRoute(Name = "provider")] string provider,
            [FromRoute(Name = "id")] string serviceDefinitionId,
            [FromBody] Parameters parameters,
            [FromQuery] bool demographicsConsentGiven,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (parameters != null)
                {
                    return await EvaluateServiceDefinition(provider, serviceDefinitionId, ServiceDefinitionType.Unknown,
                        userSession, parameters, demographicsConsentGiven);
                }

                _logger.LogError("Parameters cannot be null");

                return new ServiceDefinitionResult.BadRequest().Accept(new ServiceDefinitionResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}/evaluate")]
        public async Task<IActionResult> EvaluateServiceDefinitionV2(
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
                    return await EvaluateServiceDefinition(provider, metaData.Id, metaData.Type, userSession, parameters, demographicsConsentGiven);
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
        [ApiVersion("1"), ApiVersionRoute("service-definition/provider-name/{provider}")]
        public IActionResult GetProviderNameV1([FromRoute(Name = "provider")] string provider)
        {
            return GetProviderDetailsV2(provider);
        }

        [HttpGet]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}/details")]
        public IActionResult GetProviderDetailsV2([FromRoute(Name = "provider")] string provider)
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
        [ApiVersion("1"), ApiVersionRoute("service-definition/{provider}/$isValid")]
        public async Task<IActionResult> GetServiceDefinitionIsValidV1(
            [FromRoute(Name = "provider")] string provider,
            [UserSession] P9UserSession userSession)
        {
            return await GetServiceDefinitionIsValidV2(provider, userSession);
        }

        [HttpGet]
        [ApiVersion("2"), ApiVersionRoute("cdss/service-definition/{provider}/isValid")]
        public async Task<IActionResult> GetServiceDefinitionIsValidV2(
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

        private async Task<IActionResult> GetServiceDefinitionById(string provider, string serviceDefinitionId, ServiceDefinitionType type, P9UserSession userSession)
        {
            var description = ClinicalDecisionSupportConstants.ServiceDefinitionDescriptions[type];

            _logger.LogInformation($"Starting online consultation for {description}. " +
                                   $"ODSCode: {userSession.GpUserSession.OdsCode}");

            return (await _service.GetServiceDefinitionById(provider, serviceDefinitionId, description, userSession))
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
                                   $"ODSCode: {userSession.GpUserSession.OdsCode}");

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