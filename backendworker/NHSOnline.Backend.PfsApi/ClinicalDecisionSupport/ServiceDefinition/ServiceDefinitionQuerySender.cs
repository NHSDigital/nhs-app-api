extern alias stu3;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Extensions;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support.Sanitization;
using STU3Models = stu3::Hl7.Fhir.Model;
using STU3Serialization = stu3::Hl7.Fhir.Serialization;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    internal sealed class ServiceDefinitionQuerySender
    {
        private readonly ILogger _logger;
        private readonly IAuditor _auditor;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IFhirSanitizationHelper _fhirSanitizationHelper;
        private readonly IFhirParameterHelpers _fhirParameterHelpers;

        private readonly STU3Serialization.FhirJsonParser _parser;
        private readonly STU3Serialization.FhirJsonSerializer _serializer;
        private readonly IEvaluateServiceDefinitionQuery _evaluateServiceDefinitionQuery;
        private readonly IServiceDefinitionIsValidQuery _serviceDefinitionIsValidQuery;

        private const string AuditType = AuditingOperations.OnlineConsultationsSubmitted;

        public ServiceDefinitionQuerySender(
            ILogger<ServiceDefinitionQuerySender> logger,
            IAuditor auditor,
            IHtmlSanitizer htmlSanitizer,
            IFhirSanitizationHelper fhirSanitizationHelper,
            IFhirParameterHelpers fhirParameterHelpers,
            IEvaluateServiceDefinitionQuery evaluateServiceDefinitionQuery,
            IServiceDefinitionIsValidQuery serviceDefinitionIsValidQuery)
        {
            _logger = logger;
            _auditor = auditor;
            _htmlSanitizer = htmlSanitizer;
            _fhirSanitizationHelper = fhirSanitizationHelper;
            _fhirParameterHelpers = fhirParameterHelpers;

            _serializer = new STU3Serialization.FhirJsonSerializer();
            _parser = new STU3Serialization.FhirJsonParser();

            _evaluateServiceDefinitionQuery = evaluateServiceDefinitionQuery;
            _serviceDefinitionIsValidQuery = serviceDefinitionIsValidQuery;
        }

        public async Task<ServiceDefinitionIsValidResult> SendIsValidQueryAndHandleResponse(
            string providerKey,
            string requestBody)
        {
            HttpResponseMessage responseMessage;
            Parameters parameters;

            try
            {
                responseMessage = await _serviceDefinitionIsValidQuery.ServiceDefinitionIsValid(
                    providerKey,
                    requestBody);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Error sending request to provider");

                return new ServiceDefinitionIsValidResult.BadRequest();
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Provider responded with status code: {responseMessage.StatusCode}");

                return new ServiceDefinitionIsValidResult.BadGateway();
            }

            var stringResponse = responseMessage.Content?.ReadAsStringAsync().Result;

            try
            {
                parameters = _parser.Parse<Parameters>(stringResponse);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Received null content from provider");

                return new ServiceDefinitionIsValidResult.BadGateway();
            }
            catch (FormatException e)
            {
                _logger.LogError(e, "Failed to parse parameters response");

                return new ServiceDefinitionIsValidResult.BadGateway();
            }

            var isValid = GetReturnBooleanFromParameters(parameters);

            if (isValid.HasValue)
            {
                if (isValid.Value)
                {
                    return new ServiceDefinitionIsValidResult.Valid();
                }
                return new ServiceDefinitionIsValidResult.Invalid();
            }

            _logger.LogInformation("Unable to retrieve validity from provider response");

            return new ServiceDefinitionIsValidResult.BadGateway();
        }

        public async Task<ServiceDefinitionResult> SendEvaluateQueryAndHandleResponse(
            string providerKey,
            string serviceDefinitionId,
            string serviceDefinitionDescription,
            string parameters,
            bool addJavascriptDisabledHeader,
            string odsCode,
            string nhsLoginId,
            string version,
            string sessionId = null)
        {
            HttpResponseMessage responseMessage;
            STU3Models.GuidanceResponse guidanceResponse;

            try
            {
                responseMessage = await _evaluateServiceDefinitionQuery.EvaluateServiceDefinition(
                    providerKey,
                    serviceDefinitionId,
                    parameters,
                    addJavascriptDisabledHeader,
                    version,
                    sessionId);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Error sending request to provider");

                return new ServiceDefinitionResult.BadRequest();
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Provider responded with status code: {responseMessage.StatusCode}");

                try
                {
                    var reason =
                        JsonConvert.DeserializeObject<ErrorResponse>(
                            await responseMessage.Content.ReadAsStringAsync());
                    _logger.LogError(reason != null
                        ? $"Reason: {reason.Message}"
                        : "Unable to determine reason - empty response body returned from provider");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to determine reason - null response body returned from provider");
                }

                return new ServiceDefinitionResult.BadGateway();
            }

            var stringResponse = responseMessage.Content?.ReadAsStringAsync().Result;

            try
            {
                guidanceResponse = _parser.Parse<STU3Models.GuidanceResponse>(stringResponse);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Received null content from provider");

                return new ServiceDefinitionResult.BadGateway();
            }
            catch (FormatException e)
            {
                _logger.LogError(e, "Failed to parse guidance response");

                return new ServiceDefinitionResult.BadGateway();
            }

            if (string.IsNullOrEmpty(sessionId))
            {
                var responseSessionId = _fhirParameterHelpers.GetSessionIdFromParameters(guidanceResponse.Contained?
                    .Where(c => c.TryDeriveResourceType(out var resourceType)
                                 && resourceType == STU3Models.ResourceType.Parameters)
                    .Cast<Parameters>()
                    .FirstOrDefault());

                if (!string.IsNullOrEmpty(responseSessionId))
                {
                    _logger.LogInformation($"Starting online consultation for {serviceDefinitionDescription}. ODSCode: {odsCode}");
                }
            }

            _fhirSanitizationHelper.SanitizeGuidanceResponse(guidanceResponse, _htmlSanitizer);

            if (guidanceResponse.Status == STU3Models.GuidanceResponse.GuidanceResponseStatus.Failure)
            {
                _logger.LogInformation($"Ending consultation with failure status for {serviceDefinitionDescription}. ODSCode: {odsCode}");
                return GetServiceDefinitionResultFromErrorCode(guidanceResponse);
            }

            if (guidanceResponse.Status == STU3Models.GuidanceResponse.GuidanceResponseStatus.Success)
            {
                _logger.LogInformation(
                    $"Ending consultation for {serviceDefinitionDescription}. ODSCode: {odsCode}");

                var eConsultEndedMessage = !string.IsNullOrEmpty(sessionId)
                    ? $"eConsult successfully submitted: nhsLoginId={nhsLoginId} eConsultId={sessionId}"
                    : $"eConsult submitted before eConsultId has been assigned: nhsLoginId={nhsLoginId}";

                _logger.LogInformation(eConsultEndedMessage);
                await _auditor.PostOperationAudit(AuditType, eConsultEndedMessage);
            }

            return new ServiceDefinitionResult.Success(_serializer.SerializeToString(guidanceResponse));
        }

        private bool? GetReturnBooleanFromParameters(Parameters parameters)
        {
            try
            {
                return parameters?.Parameter?
                    .Where(p => "return".Equals(p?.Name, StringComparison.Ordinal))
                    .Select(p => p?.Value)
                    .Cast<FhirBoolean>()
                    .Select(p => p?.Value)
                    .FirstOrDefault();
            }
            catch (InvalidCastException e)
            {
                _logger.LogError(e, $"Parameter return was not of expected type: {nameof(FhirBoolean)}");
                return null;
            }
        }

        private ServiceDefinitionResult GetServiceDefinitionResultFromErrorCode(DomainResource guidanceResponse)
        {
            if (guidanceResponse
                .ExtractOperationOutcomes()
                .ExtractNotFoundOutcomes()
                .IsSessionEnded())
            {
                return new ServiceDefinitionResult.CustomError(Constants.ErrorCodes.SessionEndErrorCode);
            }

            return new ServiceDefinitionResult.Success(_serializer.SerializeToString(guidanceResponse));
        }
    }
}