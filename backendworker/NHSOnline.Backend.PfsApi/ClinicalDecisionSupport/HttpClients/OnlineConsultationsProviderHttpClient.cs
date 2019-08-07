using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public class OnlineConsultationsProviderHttpClient : IOnlineConsultationsProviderHttpClient
    {
        private readonly ILogger<OnlineConsultationsProviderHttpClient> _logger;
        private readonly AuthenticationHeaderValue AuthorizationBearer;
        
        private HttpClient Client { get; }

        public OnlineConsultationsProviderHttpClient(
            HttpClient client,
            OnlineConsultationsProviderSettings provider,
            ILogger<OnlineConsultationsProviderHttpClient> logger)
        {
            _logger = logger;
            Client = client;

            Client.BaseAddress = new Uri(provider.BaseAddress);

            AuthorizationBearer = AuthenticationHeaderValue.Parse(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Constants.HttpRequestHeaders.AuthorizationFormat,
                    provider.BearerToken));
        }

        public async Task<HttpResponseMessage> GetServiceDefinitionById(string serviceDefinitionId)
        {
            try
            {
                _logger.LogEnter();
                
                var path = string.Format(
                    CultureInfo.InvariantCulture,
                    Constants.CdsApiEndpoints.GetServiceDefinitionByIdPathFormat,
                    serviceDefinitionId);

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, path)
                {
                    Headers =
                    {
                        Authorization = AuthorizationBearer
                    }
                };

                _logger.LogInformation($"Searching for Service Definition with id: {serviceDefinitionId}");

                return await Client.SendAsync(requestMessage);
            }
            finally
            {
                _logger.LogExit();
            }

        }

        public async Task<HttpResponseMessage> SearchServiceDefinitionsByQuery()
        {
            try
            {
                _logger.LogEnter();

                var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, Constants.CdsApiEndpoints.ServiceDefinitionPath)
                    {
                        Headers =
                        {
                            Authorization = AuthorizationBearer
                        }
                    };

                return await Client.SendAsync(requestMessage);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<HttpResponseMessage> EvaluateServiceDefinition(string serviceDefinitionId, string requestBody)
        {
            try
            {
                _logger.LogEnter();

                var path = string.Format(
                    CultureInfo.InvariantCulture,
                    Constants.CdsApiEndpoints.EvaluateServiceDefinitionPathFormat,
                    serviceDefinitionId);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, path)
                {
                    Content = new StringContent(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json),
                    Headers =
                    {
                        Authorization = AuthorizationBearer
                    }
                };

                return await Client.SendAsync(requestMessage);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}