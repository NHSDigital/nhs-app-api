using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
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

        public ServiceDefinitionService(
            ILogger<ServiceDefinitionService> logger,
            IHtmlSanitizer htmlSanitizer,
            IFhirSanitizationHelper fhirSanitizationHelper)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
            _fhirSanitizationHelper = fhirSanitizationHelper;

            _serializer = new FhirJsonSerializer();
            _parser = new FhirJsonParser();
        }

        public async Task<ServiceDefinitionResult> GetServiceDefinitionById(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId)
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

                return new ServiceDefinitionResult.Success(_serializer.SerializeToString(serviceDefinition));

            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<ServiceDefinitionResult> SearchServiceDefinitionsByQuery(IOnlineConsultationsProviderHttpClient httpClient)
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

                    return new ServiceDefinitionResult.BadRequest();
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Supplier responded with status code: {responseMessage.StatusCode}");

                    return new ServiceDefinitionResult.BadGateway();
                }
                
                _logger.LogInformation($"Supplier responded with status code: {responseMessage.StatusCode}");

                var stringResponse = responseMessage.Content != null
                    ? await responseMessage.Content.ReadAsStringAsync()
                    : null;

                try
                {
                    bundle = _parser.Parse<Bundle>(stringResponse);
                }
                catch (ArgumentNullException)
                {
                    _logger.LogError("Received null content from provider");

                    return new ServiceDefinitionResult.BadGateway();
                }
                catch (FormatException fe)
                {
                    _logger.LogError($"Failed to parse search result bundle: {fe.Message}");

                    return new ServiceDefinitionResult.BadGateway();
                }
                
                _fhirSanitizationHelper.SanitizeServiceDefinitionSearchBundle(bundle, _htmlSanitizer);
                
                return new ServiceDefinitionResult.Success(_serializer.SerializeToString(bundle));
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
            bool addJavascriptDisabledHeader)
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

                return new ServiceDefinitionResult.Success(_serializer.SerializeToString(guidanceResponse));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}