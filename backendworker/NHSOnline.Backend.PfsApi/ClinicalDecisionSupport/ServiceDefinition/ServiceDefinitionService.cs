using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Mappers;
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
        private readonly IServiceDefinitionListBuilder _serviceDefinitionListBuilder;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IFhirSanitizationHelper _fhirSanitizationHelper;
        
        private readonly FhirJsonParser _parser;
        private readonly FhirJsonSerializer _serializer;
        private readonly IMapper<DemographicsResponse, OlcDemographics> _demographicsOlcMapper;
        private readonly IOlcDataMaps _olcDataMapper;
        private readonly IAuditor _auditor;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly OnlineConsultationsProvidersSettings _olcProvidersSettings;
        private readonly ICreateFhirParameter _createFhirParameter;

        public ServiceDefinitionService(
            ILogger<ServiceDefinitionService> logger,
            IServiceDefinitionListBuilder serviceDefinitionListBuilder,
            IHtmlSanitizer htmlSanitizer,
            IFhirSanitizationHelper fhirSanitizationHelper,
            IMapper<DemographicsResponse, OlcDemographics> demographicsRegistrationMapper,
            IOlcDataMaps dataMaps, IAuditor auditor, IGpSystemFactory gpSystemFactory,
            OnlineConsultationsProvidersSettings olcProvidersSettings,
            ICreateFhirParameter createFhirParameter)
        {
            _logger = logger;
            _serviceDefinitionListBuilder = serviceDefinitionListBuilder;
            _htmlSanitizer = htmlSanitizer;
            _fhirSanitizationHelper = fhirSanitizationHelper;

            _serializer = new FhirJsonSerializer();
            _parser = new FhirJsonParser();
            
            _demographicsOlcMapper = demographicsRegistrationMapper;
            _olcDataMapper = dataMaps;

            _auditor = auditor;
            
            _gpSystemFactory = gpSystemFactory;

            _olcProvidersSettings = olcProvidersSettings;
            _createFhirParameter = createFhirParameter;
        }

        public async Task<ServiceDefinitionResult> GetServiceDefinitionById(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, string provider)
        {
            try
            {
                HttpResponseMessage responseMessage;
                Hl7.Fhir.Model.ServiceDefinition serviceDefinition;

                _logger.LogEnter();

                _logger.LogInformation($"Searching for Service Definition with id: {serviceDefinitionId}");

                try
                {
                    responseMessage = await httpClient.GetServiceDefinitionById(serviceDefinitionId);
                }
                catch (HttpRequestException hre)
                {
                    _logger.LogError($"Error sending request to CDSS supplier: {hre.Message}");

                    return new ServiceDefinitionResult.BadRequest();
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Supplier responded with status code: {responseMessage.StatusCode}");

                    if (!responseMessage.StatusCode.Equals(HttpStatusCode.NotFound))
                    {
                        return new ServiceDefinitionResult.BadGateway();
                    }

                    _logger.LogError($"No service definition found for id: {serviceDefinitionId}");

                    return new ServiceDefinitionResult.NotFound();
                }

                _logger.LogInformation($"Supplier responded with status code: {responseMessage.StatusCode}");

                var stringResponse = responseMessage.Content != null
                    ? await responseMessage.Content.ReadAsStringAsync()
                    : null;

                try
                {
                    serviceDefinition = _parser.Parse<Hl7.Fhir.Model.ServiceDefinition>(stringResponse);
                }
                catch (ArgumentNullException)
                {
                    _logger.LogError("Received null content from provider");

                    return new ServiceDefinitionResult.BadGateway();
                }
                catch (FormatException fe)
                {
                    _logger.LogError($"Failed to parse service definition: {fe.Message}");

                    return new ServiceDefinitionResult.BadGateway();
                }

                _fhirSanitizationHelper.SanitizeServiceDefinition(serviceDefinition, _htmlSanitizer);
                
                var providerSettings = _olcProvidersSettings.getProvider(provider);
                foreach (var child in serviceDefinition.Children)
                {

                    if (child.TypeName.Equals("Questionnaire",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var questionnaireElement in ((Questionnaire) child).Children)
                        {
                            if (questionnaireElement.TypeName.Equals(new Questionnaire.ItemComponent().TypeName,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                var linkIdElement = new FhirString { Value = Support.Constants.OnlineConsultationsConstants.DemographicsOptionCode };
                                var textElement = new FhirString
                                {
                                    Value = string.Format(CultureInfo.InvariantCulture, Support.Constants.OnlineConsultationsConstants.DemographicsLabel, providerSettings.ProviderName)
                                };
                                var typeElement = new Code<Questionnaire.QuestionnaireItemType>
                                {
                                    Value = Questionnaire.QuestionnaireItemType.Boolean
                                };
                                var required = new FhirBoolean { Value = false };
                                var newAnswer = new Questionnaire.ItemComponent
                                {
                                    LinkIdElement = linkIdElement,
                                    TextElement = textElement,
                                    TypeElement = typeElement,
                                    RequiredElement = required
                                };
                                ((Questionnaire.ItemComponent) questionnaireElement).Item.Add(newAnswer);
                            }
                        }
                    }
                }
                return new ServiceDefinitionResult.Success(_serializer.SerializeToString(serviceDefinition));

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

                var addPatient = false;
                foreach (var param in parameters.Parameter)
                {
                    if (param.Resource?.Children != null)
                    {
                        foreach (var resource in (param.Resource).Children)
                        {
                            if (resource.TypeName.Equals(new QuestionnaireResponse.ItemComponent().TypeName,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                QuestionnaireResponse.AnswerComponent answerToRemove = null ;
                                foreach (var answer in ((QuestionnaireResponse.ItemComponent) resource).Answer)
                                {
                                    if (answer.Value.TypeName.Equals(new Coding().TypeName,
                                            StringComparison.OrdinalIgnoreCase) && ((Coding) answer.Value).CodeElement.Value.Equals(
                                            Support.Constants.OnlineConsultationsConstants
                                                .DemographicsOptionCode,
                                            StringComparison.OrdinalIgnoreCase))
                                    {
                                        addPatient = true;
                                        answerToRemove = answer;
                                    }
                                }

                                if (answerToRemove != null)
                                {
                                    ((QuestionnaireResponse.ItemComponent) resource).Answer.Remove(answerToRemove);
                                }
                            }
                        }
                            
                    }
                }
                
                if (addPatient)
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
                        "User has agreed to share their name, age, gender, NHS number and postal address.");
                }
                else
                {
                    await _auditor.Audit(
                        AuditingOperations.OnlineConsultationsDemographicAuditTypeRequest,
                        "User has not agreed to share their name, age, gender, NHS number and postal address.");
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