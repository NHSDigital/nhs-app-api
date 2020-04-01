using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Extensions;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public class ServiceDefinitionService: IServiceDefinitionService
    {
        private readonly ILogger<ServiceDefinitionService> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IFhirSanitizationHelper _fhirSanitizationHelper;
        
        private readonly FhirJsonParser _parser;
        private readonly FhirJsonSerializer _serializer;
        private readonly IMapper<DemographicsResponse, OlcDemographics> _demographicsOlcMapper;
        private readonly IAuditor _auditor;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ICreateFhirParameter _createFhirParameter;
        private readonly OnlineConsultationsProvidersSettings _olcProvidersSettings;
        private readonly IEvaluateServiceDefinitionQuery _evaluateServiceDefinitionQuery;
        private readonly IServiceDefinitionIsValidQuery _serviceDefinitionIsValidQuery;
        private readonly IGuidCreator _guidCreator;

        public ServiceDefinitionService(
            ILogger<ServiceDefinitionService> logger,
            IHtmlSanitizer htmlSanitizer,
            IFhirSanitizationHelper fhirSanitizationHelper,
            IMapper<DemographicsResponse, OlcDemographics> demographicsRegistrationMapper,
            IAuditor auditor, 
            IGpSystemFactory gpSystemFactory,
            ICreateFhirParameter createFhirParameter,
            OnlineConsultationsProvidersSettings olcProvidersSettings,
            IEvaluateServiceDefinitionQuery evaluateServiceDefinitionQuery,
            IServiceDefinitionIsValidQuery serviceDefinitionIsValidQuery,
            IGuidCreator guidCreator)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
            _fhirSanitizationHelper = fhirSanitizationHelper;
            _guidCreator = guidCreator;

            _serializer = new FhirJsonSerializer();
            _parser = new FhirJsonParser();
            
            _demographicsOlcMapper = demographicsRegistrationMapper;

            _auditor = auditor;
            
            _gpSystemFactory = gpSystemFactory;
            _createFhirParameter = createFhirParameter;

            _olcProvidersSettings = olcProvidersSettings;
            _evaluateServiceDefinitionQuery = evaluateServiceDefinitionQuery;
            _serviceDefinitionIsValidQuery = serviceDefinitionIsValidQuery;
        }

        public async Task<ServiceDefinitionResult> GetServiceDefinitionById(string providerKey,
            string serviceDefinitionId, UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogInformation($"Initial evaluation for Service Definition with id: {serviceDefinitionId}");
                
                var odsCode = userSession.GpUserSession.OdsCode;

                var requestBody = "{ \"resourceType\":\"Parameters\", \"meta\":{ \"profile\":[ " +
                                 "\"http://hl7.org/fhir/OperationDefinition/ServiceDefinition-evaluate\" ] }, " +
                                 "\"parameter\": [ { \"name\": \"organization\", \"resource\": { \"resourceType\": " +
                                 "\"Organization\", \"identifier\": { \"value\": \"" + odsCode + "\" }}}]}";

                return await SendEvaluateQueryAndHandleResponse(
                    providerKey,
                    serviceDefinitionId,
                    requestBody,
                    false,
                    odsCode);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public ServiceDefinitionResult GetProviderName(string providerKey)
        {
            _logger.LogEnter();

            var provider = _olcProvidersSettings.Providers
                .FirstOrDefault(a => a.Provider.Equals(providerKey, StringComparison.Ordinal));

            if (provider == null) {
                return new ServiceDefinitionResult.NotFound();
            }

            _logger.LogExit();

            return new ServiceDefinitionResult.Success(provider.ProviderName);
        }

        public async Task<ServiceDefinitionResult> EvaluateServiceDefinition(
            string providerKey,
            string serviceDefinitionId,
            Parameters parameters,
            bool addJavascriptDisabledHeader,
            bool demographicsConsentGiven,
            UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (parameters == null)
                {
                    _logger.LogError("Parameters can not be null");

                    return new ServiceDefinitionResult.BadRequest();
                }
                
                if (demographicsConsentGiven)
                {
                    _logger.LogInformation($"Fetching DemographicsService for supplier: {userSession.GpUserSession.Supplier.ToString()}");

                    var demographicsService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                        .GetDemographicsService();

                    _logger.LogDebug("Fetching Demographics");
                    var demographics = await demographicsService.GetDemographics(
                        new GpLinkedAccountModel(userSession.GpUserSession));
                            
                    if (!(demographics is DemographicsResult.Success demographicsResult))
                    {
                        return GetDemographicsErrorResult(demographics);
                    }

                    parameters.Add("patient", _createFhirParameter.CreatePatientFhir(_demographicsOlcMapper, demographicsResult));
                    await _auditor.Audit(
                        AuditingOperations.OnlineConsultationsDemographicAuditTypeRequest,
                        "User has agreed to share their name, age, NHS number and postal address.");
                }
                else
                {
                    await _auditor.Audit(
                        AuditingOperations.OnlineConsultationsDemographicAuditTypeRequest,
                        "User has not agreed to share their name, age, NHS number and postal address.");
                }

                return await SendEvaluateQueryAndHandleResponse(
                    providerKey,
                    serviceDefinitionId,
                    _serializer.SerializeToString(parameters),
                    addJavascriptDisabledHeader,
                    userSession.GpUserSession.OdsCode,
                    GetSessionIdFromParameters(parameters));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<ServiceDefinitionIsValidResult> GetServiceDefinitionIsValid(string providerKey, UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                
                var odsCode = userSession.GpUserSession.OdsCode;
                
                _logger.LogInformation($"Checking if online consultations are enabled for practice: {odsCode}");

                var requestId = _guidCreator.CreateGuid().ToString();
                
                _logger.LogInformation($"$isValid requestId: {requestId}");
                
                var parameters = new Parameters
                {
                    Parameter = new List<Parameters.ParameterComponent>
                    {
                        new Parameters.ParameterComponent
                        {
                            Name = "ODSCode",
                            Value = new FhirString(odsCode)
                        },
                        new Parameters.ParameterComponent
                        {
                            Name = "requestId",
                            Value = new FhirString(requestId)
                        }
                    }
                };
                
                return await SendIsValidQueryAndHandleResponse(
                    providerKey,
                    _serializer.SerializeToString(parameters));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<ServiceDefinitionIsValidResult> SendIsValidQueryAndHandleResponse(
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

        private async Task<ServiceDefinitionResult> SendEvaluateQueryAndHandleResponse(
            string providerKey,
            string serviceDefinitionId,
            string requestBody,
            bool addJavascriptDisabledHeader,
            string odsCode,
            string sessionId = null)
        {
            HttpResponseMessage responseMessage;
            GuidanceResponse guidanceResponse;
            
            try
            {
                responseMessage = await _evaluateServiceDefinitionQuery.EvaluateServiceDefinition(
                    providerKey,
                    serviceDefinitionId,
                    requestBody,
                    addJavascriptDisabledHeader,
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
                guidanceResponse = _parser.Parse<GuidanceResponse>(stringResponse);
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

            _fhirSanitizationHelper.SanitizeGuidanceResponse(guidanceResponse, _htmlSanitizer);

            if (guidanceResponse.Status == GuidanceResponse.GuidanceResponseStatus.Failure)
            {
                _logger.LogInformation($"Ending consultation with failure status for ServiceDefinition: {serviceDefinitionId}. ODSCode: {odsCode}");
                return GetServiceDefinitionResultFromErrorCode(guidanceResponse);
            }

            if (guidanceResponse.Status == GuidanceResponse.GuidanceResponseStatus.Success)
            {
                _logger.LogInformation(
                    $"Ending consultation with ServiceDefinition: {serviceDefinitionId}. ODSCode: {odsCode}");
            }

            return new ServiceDefinitionResult.Success(_serializer.SerializeToString(guidanceResponse));
        }
        
        private ServiceDefinitionResult GetDemographicsErrorResult(DemographicsResult myRecord)
        {
            switch (myRecord)
            {
                case DemographicsResult.BadGateway _:
                    _logger.LogError("GP systems demographics call was unsuccessful");
                    return new ServiceDefinitionResult.DemographicsBadGateway();
                case DemographicsResult.Forbidden _:
                    _logger.LogError("GP systems demographics forbidden");
                    return new ServiceDefinitionResult.DemographicsForbidden();
                case DemographicsResult.InternalServerError _:
                    _logger.LogError("GP systems demographics threw an internal server error");
                    return new ServiceDefinitionResult.DemographicsInternalServerError();
                default:
                    _logger.LogError("GP systems demographics record not successfully retrieved");
                    return new ServiceDefinitionResult.DemographicsRetrievalFailed();
            }
        }

        private string GetSessionIdFromParameters(Parameters parameters)
        {
            try
            {
                return parameters?.Parameter?
                    .Where(p => "sessionId".Equals(p?.Name, StringComparison.Ordinal))
                    .Select(p => p?.Value)
                    .Cast<FhirString>()
                    .Select(p => p?.Value)
                    .FirstOrDefault();
            }
            catch (InvalidCastException e)
            {
                _logger.LogError(e, $"Parameter sessionId was not of expected type: {nameof(FhirString)}");
                return null;
            }
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
                    return
                        new ServiceDefinitionResult.CustomError(Constants.ErrorCodes.SessionEndErrorCode);
                }

            return new ServiceDefinitionResult.Success(_serializer.SerializeToString(guidanceResponse));
        }
    }
}