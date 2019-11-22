using System;
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

        public ServiceDefinitionService(
            ILogger<ServiceDefinitionService> logger,
            IHtmlSanitizer htmlSanitizer,
            IFhirSanitizationHelper fhirSanitizationHelper,
            IMapper<DemographicsResponse, OlcDemographics> demographicsRegistrationMapper,
            IAuditor auditor, 
            IGpSystemFactory gpSystemFactory,
            ICreateFhirParameter createFhirParameter,
            OnlineConsultationsProvidersSettings olcProvidersSettings,
            IEvaluateServiceDefinitionQuery evaluateServiceDefinitionQuery)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
            _fhirSanitizationHelper = fhirSanitizationHelper;

            _serializer = new FhirJsonSerializer();
            _parser = new FhirJsonParser();
            
            _demographicsOlcMapper = demographicsRegistrationMapper;

            _auditor = auditor;
            
            _gpSystemFactory = gpSystemFactory;
            _createFhirParameter = createFhirParameter;

            _olcProvidersSettings = olcProvidersSettings;
            _evaluateServiceDefinitionQuery = evaluateServiceDefinitionQuery;
        }

        public async Task<ServiceDefinitionResult> GetServiceDefinitionById(string providerKey,
            string serviceDefinitionId, UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogInformation($"Initial evaluation for Service Definition with id: {serviceDefinitionId}");

                var requestBody = "{ \"resourceType\":\"Parameters\", \"meta\":{ \"profile\":[ " +
                                 "\"http://hl7.org/fhir/OperationDefinition/ServiceDefinition-evaluate\" ] }, " +
                                 "\"parameter\": [ { \"name\": \"organization\", \"resource\": { \"resourceType\": " +
                                 "\"Organization\", \"identifier\": { \"value\": \"" +
                                 userSession.GpUserSession.OdsCode + "\" }}}]}";

                return await SendEvaluateQueryAndHandleResponse(
                    providerKey,
                    serviceDefinitionId,
                    requestBody,
                    false);
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
                    addJavascriptDisabledHeader);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<ServiceDefinitionResult> SendEvaluateQueryAndHandleResponse(
            string providerKey,
            string serviceDefinitionId,
            string requestBody,
            bool addJavascriptDisabledHeader)
        {
            HttpResponseMessage responseMessage;
            GuidanceResponse guidanceResponse;
            
            try
            {
                responseMessage = await _evaluateServiceDefinitionQuery.EvaluateServiceDefinition(
                    providerKey,
                    serviceDefinitionId,
                    requestBody,
                    addJavascriptDisabledHeader);
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

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;

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

            if (guidanceResponse.Status == GuidanceResponse.GuidanceResponseStatus.Success)
            {
                _logger.LogInformation($"Ending consultation with ServiceDefinition: {serviceDefinitionId}");
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
    }
}