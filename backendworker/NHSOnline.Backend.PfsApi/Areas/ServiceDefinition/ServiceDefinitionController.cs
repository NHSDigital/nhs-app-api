using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceDefinition
{
    [ApiController]
    public class ServiceDefinitionController : Controller
    {
        private readonly IServiceDefinitionService _service;
        private readonly ILogger<ServiceDefinitionController> _logger;
        private readonly IOnlineConsultationsProviderHttpClientPool _onlineConsultationsProviderHttpClientPool;

        public ServiceDefinitionController(
            IServiceDefinitionService service,
            ILoggerFactory loggerFactory,
            IOnlineConsultationsProviderHttpClientPool onlineConsultationsProviderHttpClientPool)
        {
            _service = service;
            _logger = loggerFactory.CreateLogger<ServiceDefinitionController>();
            _onlineConsultationsProviderHttpClientPool = onlineConsultationsProviderHttpClientPool;
        }
        
        [HttpGet]
        [Route("fhir/ServiceDefinition")]
        public async Task<IActionResult> SearchServiceDefinitionsByQuery([FromQuery(Name = "_provider")] string provider)
        {
            try
            {
                _logger.LogEnter();

                var visitor = new ServiceDefinitionResultVisitor();

                if (string.IsNullOrWhiteSpace(provider))
                {
                  _logger.LogError("Missing provider in route");
                  return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                var httpClient =
                    _onlineConsultationsProviderHttpClientPool.GetClientByProviderName(
                        provider);

                if (httpClient == null)
                {
                    _logger.LogError($"No http client found for provider {provider}");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                var result = await _service.SearchServiceDefinitionsByQuery(httpClient);

                return result.Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Route("fhir/ServiceDefinition/{provider}/{id}")]
        public async Task<IActionResult> GetServiceDefinitionsById([FromRoute(Name = "id")] string serviceDefinitionId, 
          [FromRoute(Name = "provider")] string provider)
        {
            try
            {
                _logger.LogEnter();

                var visitor = new ServiceDefinitionResultVisitor();

                if (string.IsNullOrWhiteSpace(provider))
                {
                  _logger.LogError("Missing provider in route");
                  return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                if (string.IsNullOrWhiteSpace(serviceDefinitionId))
                {
                  _logger.LogError("Missing serviceDefinition in route");
                  return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                var httpClient =
                    _onlineConsultationsProviderHttpClientPool.GetClientByProviderName(
                        provider);

                if (httpClient == null)
                {
                    _logger.LogError($"No http client found for provider {provider}");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                var result = await _service.GetServiceDefinitionById(httpClient, serviceDefinitionId);

                return result.Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [Route("fhir/ServiceDefinition/{provider}/{id}/$evaluate")]
        public async Task<IActionResult> EvaluateServiceDefinition([FromRoute(Name = "provider")] string provider, 
          [FromRoute(Name = "id")] string serviceDefinitionId, [FromBody] Parameters parameters)
        {
            try
            {
                var visitor = new ServiceDefinitionResultVisitor();

                if (string.IsNullOrWhiteSpace(provider))
                {
                  _logger.LogError("Missing provider in querystring");
                  return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }


                if (string.IsNullOrWhiteSpace(serviceDefinitionId))
                {
                  _logger.LogError("Missing service definition in querystring");
                  return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }
                
                _logger.LogEnter();

                if (parameters == null)
                {
                    _logger.LogError("Parameters cannot be null");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                var httpClient =
                    _onlineConsultationsProviderHttpClientPool.GetClientByProviderName(
                        provider);

                if (httpClient == null)
                {
                    _logger.LogError($"No http client found for provider {provider}");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                _logger.LogInformation($"Evaluating ServiceDefinition: {serviceDefinitionId}");

                return (await _service.EvaluateServiceDefinition(httpClient, serviceDefinitionId, parameters))
                    .Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}