using System;
using System.Net.Http;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public class ServiceDefinitionService: IServiceDefinitionService
    {
        private readonly ILogger<ServiceDefinitionService> _logger;
        private readonly IServiceDefinitionListBuilder _serviceDefinitionListBuilder;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IFhirSanitizationHelper _fhirSanitizationHelper;
        
        private readonly FhirJsonParser _parser;
        private readonly FhirJsonSerializer _serializer;
        private readonly IMapper<DemographicsResponse, OlcDemographics> _demographicsOlcMapper;
        private readonly IAuditor _auditor;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ICreateFhirParameter _createFhirParameter;

        public ServiceDefinitionService(
            ILogger<ServiceDefinitionService> logger,
            IServiceDefinitionListBuilder serviceDefinitionListBuilder,
            IHtmlSanitizer htmlSanitizer,
            IFhirSanitizationHelper fhirSanitizationHelper,
            IMapper<DemographicsResponse, OlcDemographics> demographicsRegistrationMapper,
            IAuditor auditor, 
            IGpSystemFactory gpSystemFactory,
            ICreateFhirParameter createFhirParameter)
        {
            _logger = logger;
            _serviceDefinitionListBuilder = serviceDefinitionListBuilder;
            _htmlSanitizer = htmlSanitizer;
            _fhirSanitizationHelper = fhirSanitizationHelper;

            _serializer = new FhirJsonSerializer();
            _parser = new FhirJsonParser();
            
            _demographicsOlcMapper = demographicsRegistrationMapper;

            _auditor = auditor;
            
            _gpSystemFactory = gpSystemFactory;
            _createFhirParameter = createFhirParameter;
        }

        public async Task<ServiceDefinitionResult> GetServiceDefinitionById(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, string provider, UserSession userSession)
        {
            try
            {
                HttpResponseMessage responseMessage;
    
                _logger.LogEnter();

                _logger.LogInformation($"Searching for Service Definition with id: {serviceDefinitionId}");

                var bodyString = "{ \"resourceType\":\"Parameters\", \"meta\":{ \"profile\":[ " +
                                 "\"http://hl7.org/fhir/OperationDefinition/ServiceDefinition-evaluate\" ] }, " +
                                 "\"parameter\": [ { \"name\": \"organization\", \"resource\": { \"resourceType\": " +
                                 "\"Organization\", \"identifier\": { \"value\": \"" +
                                 userSession.GpUserSession.OdsCode + "\" }}}]}";

                try
                {
                    responseMessage = await httpClient.EvaluateServiceDefinition(serviceDefinitionId,
                        bodyString, false);
                }
                catch (HttpRequestException hre)
                {
                    _logger.LogError($"Error sending request to CDSS supplier: {hre.Message}");

                    return new ServiceDefinitionResult.BadRequest();
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Supplier responded with status code: {responseMessage.StatusCode}");

                    return new ServiceDefinitionResult.BadGateway();
                }

                var stringResponse = responseMessage.Content != null
                    ? await responseMessage.Content.ReadAsStringAsync()
                    : null;

                GuidanceResponse guidanceResponse;
                
                try
                {
                    guidanceResponse = _parser.Parse<GuidanceResponse>(stringResponse);
                }
                catch (ArgumentNullException)
                {
                    _logger.LogError("Received null content from provider");

                    return new ServiceDefinitionResult.BadGateway();
                }
                catch (FormatException fe)
                {
                    _logger.LogError($"Failed to parse guidance response: {fe.Message}");

                    return new ServiceDefinitionResult.BadGateway();
                }
                
                _fhirSanitizationHelper.SanitizeGuidanceResponse(guidanceResponse, _htmlSanitizer);

                if (guidanceResponse.Status == GuidanceResponse.GuidanceResponseStatus.Success)
                {
                    _logger.LogInformation($"Ending consultation with ServiceDefinition: {serviceDefinitionId}");
                }

                return new ServiceDefinitionResult.Success(_serializer.SerializeToString(guidanceResponse));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<ServiceDefinitionListResult> GetServiceDefinitions(IOnlineConsultationsProviderHttpClient httpClient)
        {
            try
            {
                HttpResponseMessage responseMessage;
                Bundle bundle;
                
                _logger.LogEnter();

                try
                {
                    responseMessage = await httpClient.SearchServiceDefinitionsByQuery();
                }
                catch (HttpRequestException hre)
                {
                    _logger.LogError($"Error sending request to CDSS supplier: {hre.Message}");

                    return new ServiceDefinitionListResult.BadRequest();
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Supplier responded with status code: {responseMessage.StatusCode}");

                    return new ServiceDefinitionListResult.BadGateway();
                }
                
                _logger.LogInformation($"Supplier responded with status code: {responseMessage.StatusCode}");

                var stringResponse = responseMessage.Content != null
                    ? await responseMessage.Content.ReadAsStringAsync()
                    : null;

                _logger.LogInformation($"Supplier responded with status code: {responseMessage.StatusCode}");

                try
                {
                    bundle = _parser.Parse<Bundle>(stringResponse);
                }
                catch (ArgumentNullException)
                {
                    _logger.LogError("Received null content from provider");

                    return new ServiceDefinitionListResult.BadGateway();
                }
                catch (FormatException fe)
                {
                    _logger.LogError($"Failed to parse search result bundle: {fe.Message}");

                    return new ServiceDefinitionListResult.BadGateway();
                }
                
                var serviceDefinitionList = _serviceDefinitionListBuilder.BuildServiceDefinitionList(bundle);

                return new ServiceDefinitionListResult.Success(serviceDefinitionList);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<ServiceDefinitionResult> EvaluateServiceDefinition(
            IOnlineConsultationsProviderHttpClient httpClient,
            string serviceDefinitionId,
            Parameters parameters,
            bool addJavascriptDisabledHeader,
            bool demographicsConsentGiven,
            UserSession userSession)
        {
            try
            {
                HttpResponseMessage responseMessage;
                GuidanceResponse guidanceResponse;

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
                    var demographics = await demographicsService.GetDemographics(userSession.GpUserSession);
            
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

                try
                {
                    responseMessage = await httpClient.EvaluateServiceDefinition(serviceDefinitionId,
                        _serializer.SerializeToString(parameters), addJavascriptDisabledHeader);
                }
                catch (HttpRequestException hre)
                {
                    _logger.LogError($"Error sending request to CDSS supplier: {hre.Message}");

                    return new ServiceDefinitionResult.BadRequest();
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Supplier responded with status code: {responseMessage.StatusCode}");

                    return new ServiceDefinitionResult.BadGateway();
                }

                var stringResponse = responseMessage.Content != null
                    ? await responseMessage.Content.ReadAsStringAsync()
                    : null;

                try
                {
                    guidanceResponse = _parser.Parse<GuidanceResponse>(stringResponse);
                }
                catch (ArgumentNullException)
                {
                    _logger.LogError("Received null content from provider");

                    return new ServiceDefinitionResult.BadGateway();
                }
                catch (FormatException fe)
                {
                    _logger.LogError($"Failed to parse guidance response: {fe.Message}");

                    return new ServiceDefinitionResult.BadGateway();
                }

                _fhirSanitizationHelper.SanitizeGuidanceResponse(guidanceResponse, _htmlSanitizer);

                if (guidanceResponse.Status == GuidanceResponse.GuidanceResponseStatus.Success)
                {
                    _logger.LogInformation($"Ending consultation with ServiceDefinition: {serviceDefinitionId}");
                }

                return new ServiceDefinitionResult.Success(_serializer.SerializeToString(guidanceResponse));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private ServiceDefinitionResult GetDemographicsErrorResult(DemographicsResult myRecord)
        {
            switch (myRecord)
            {
                case DemographicsResult.BadGateway _:
                    _logger.LogDebug("GP systems demographics call was unsuccessful");
                    return new ServiceDefinitionResult.DemographicsBadGateway();
                case DemographicsResult.Forbidden _:
                    _logger.LogDebug("GP systems demographics forbidden");
                    return new ServiceDefinitionResult.DemographicsForbidden();
                case DemographicsResult.InternalServerError _:
                    _logger.LogDebug("GP systems demographics threw an internal server error");
                    return new ServiceDefinitionResult.DemographicsInternalServerError();
                default:
                    _logger.LogDebug("GP systems demographics record not successfully retrieved");
                    return new ServiceDefinitionResult.DemographicsRetrievalFailed();
            }
        }
    }
}