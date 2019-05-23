using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ClinicalDecisionSupportApi.HttpClients;
using NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition;
using NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Areas.ServiceDefinition
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

        
        // TODO: NHSO-5567 disallow anonymous
        [HttpGet]
        [Route("fhir/ServiceDefinition")]
        [AllowAnonymous]
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

        // TODO: NHSO-5567 disallow anonymous
        [HttpPost]
        [Route("fhir/ServiceDefinition/{id}/$evaluate")]
        [AllowAnonymous]
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