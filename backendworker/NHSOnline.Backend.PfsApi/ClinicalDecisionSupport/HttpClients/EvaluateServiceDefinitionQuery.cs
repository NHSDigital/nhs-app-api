using System.Globalization;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public class EvaluateServiceDefinitionQuery : IEvaluateServiceDefinitionQuery
    {
        private readonly ILogger<EvaluateServiceDefinitionQuery> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public EvaluateServiceDefinitionQuery(ILogger<EvaluateServiceDefinitionQuery> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> EvaluateServiceDefinition(
            string providerKey,
            string serviceDefinitionId,
            string requestBody,
            bool addJavascriptDisabledHeader,
            string version,
            string sessionId = null)
        {
            try
            {
                _logger.LogEnter();

                var httpClient = _httpClientFactory.CreateClient(providerKey);

                var path = string.Format(
                    CultureInfo.InvariantCulture,
                    Constants.CdsApiEndpoints.EvaluateServiceDefinitionPathFormat,
                    serviceDefinitionId);

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, path)
                {
                    Content = new StringContent(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json)
                })
                {
                    requestMessage.Headers.Add(Support.Constants.OnlineConsultationConstants.ProviderIdentifierHeader,
                        providerKey);

                    if (!string.IsNullOrWhiteSpace(sessionId))
                    {
                        requestMessage.Headers.Add(
                            Support.Constants.OnlineConsultationConstants.SessionIdentifierHeader, sessionId);
                    }

                    if (version != "1")
                    {
                        requestMessage.Headers.Add(
                            Support.Constants.OnlineConsultationConstants.ApiVersion, version);
                    }

                    if (addJavascriptDisabledHeader)
                    {
                        requestMessage.Headers.Add(Support.Constants.HttpHeaders.JavascriptDisabled, "true");
                    }

                    _logger.LogInformation("Answer Successfully posted");

                    return await httpClient.SendAsync(requestMessage);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}