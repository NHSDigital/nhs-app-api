using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public class ServiceDefinitionIsValidQuery: IServiceDefinitionIsValidQuery
    {
        private readonly ILogger<IServiceDefinitionIsValidQuery> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public ServiceDefinitionIsValidQuery(ILogger<IServiceDefinitionIsValidQuery> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<HttpResponseMessage> ServiceDefinitionIsValid(string providerKey, string requestBody)
        {
            try
            {
                _logger.LogEnter();

                var httpClient = _httpClientFactory.CreateClient(providerKey);

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.CdsApiEndpoints.ServiceDefinitionIsValidPath)
                {
                    Content = new StringContent(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json)
                })
                {
                    requestMessage.Headers.Add(Support.Constants.OnlineConsultationConstants.ProviderIdentifierHeader,
                        providerKey);

                    _logger.LogInformation("Checking with provider if service definition is valid");

                    var response = await httpClient.SendAsync(requestMessage);

                    _logger.LogInformation("Service definition validity received from provider");

                    return response;
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}