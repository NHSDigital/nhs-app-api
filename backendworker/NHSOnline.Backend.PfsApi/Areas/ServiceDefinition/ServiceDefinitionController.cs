using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport;
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
        public async Task<IActionResult> SearchServiceDefinitionsByQuery([FromQuery(Name = "_id")] string serviceDefinitionId)
        {
            try
            {
                _logger.LogEnter();

                var visitor = new ServiceDefinitionResultVisitor();

                const string Provider = Constants.OnlineConsultationsProviders.EConsult;
                
                var httpClient =
                    _onlineConsultationsProviderHttpClientPool.GetClientByProviderName(
                        Provider);

                if (httpClient == null)
                {
                    _logger.LogError($"No http client found for provider {Provider}");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                var result = string.IsNullOrWhiteSpace(serviceDefinitionId)
                    ? await _service.SearchServiceDefinitionsByQuery(httpClient)
                    : await _service.GetServiceDefinitionById(httpClient, serviceDefinitionId);

                return result.Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [Route("fhir/ServiceDefinition/{id}/$evaluate")]
        public async Task<IActionResult> EvaluateServiceDefinition([FromRoute(Name = "id")] string serviceDefinitionId, [FromBody] Parameters parameters)
        {
            try
            {
                _logger.LogEnter();

                var visitor = new ServiceDefinitionResultVisitor();

                const string Provider = Constants.OnlineConsultationsProviders.EConsult;

                if (string.IsNullOrWhiteSpace(serviceDefinitionId))
                {
                    _logger.LogError("Missing ServiceDefinition id in route");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                if (parameters == null)
                {
                    _logger.LogError("Parameters cannot be null");

                    return new ServiceDefinitionResult.BadRequest().Accept(visitor);
                }

                var httpClient =
                    _onlineConsultationsProviderHttpClientPool.GetClientByProviderName(
                        Provider);

                if (httpClient == null)
                {
                    _logger.LogError($"No http client found for provider {Provider}");

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