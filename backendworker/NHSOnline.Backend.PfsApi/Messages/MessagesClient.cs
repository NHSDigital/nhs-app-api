using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.PfsApi.Messages.Models;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class MessagesClient : IMessagesClient
    {
        private const string MediaType = "application/json";

        private readonly ILogger<MessagesClient> _logger;
        private readonly IMessagesApiConfig _configuration;
        private readonly MessagesHttpClient _httpClient;

        public MessagesClient(
            ILogger<MessagesClient> logger,
            IMessagesApiConfig configuration,
            MessagesHttpClient httpClient
        )
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<MessagesResponse> Post(AddMessageRequest messageRequest)
        {
            var resourceUri = string.Format(
                CultureInfo.InvariantCulture, 
                _configuration.ResourceUrl,
                messageRequest.SenderContext.NhsLoginId
            );

            using var request = new HttpRequestMessage(HttpMethod.Post, resourceUri)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(messageRequest),
                    Encoding.UTF8,
                    MediaType
                )
            };

            var response = await _httpClient.Client.SendAsync(request);

            return new MessagesResponse(response, _logger);
        }
    }
}
