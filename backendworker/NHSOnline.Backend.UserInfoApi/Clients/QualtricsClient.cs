using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.UserInfoApi.Clients.Models;

namespace NHSOnline.Backend.UserInfoApi.Clients
{
    internal class QualtricsClient : IUserResearchClient
    {
        private readonly QualtricsHttpClient _httpClient;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly string _path;

        public QualtricsClient(QualtricsHttpClient httpClient, IQualtricsConfig config)
        {
            _httpClient = httpClient;

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            _path = $"directories/{config.DirectoryId}/mailinglists/{config.MailingListId}/contacts";
        }

        public async Task<UserResearchClientResponse> Post(string nhsLoginId, string email, string odsCode)
        {
            var subscription = new Subscription
            {
                Email = email,
                ExtRef = nhsLoginId,
                EmbeddedData = new EmbeddedData
                {
                    NhsLoginId = nhsLoginId,
                    OdsCode = odsCode
                }
            };
            using var request = BuildRequest(HttpMethod.Post, subscription);
            return await SendRequestAndParseResponse(request);
        }

        private HttpRequestMessage BuildRequest<TRequest>(HttpMethod httpMethod, TRequest model)
        {
            var request = new HttpRequestMessage(httpMethod, _path);
            var body = JsonConvert.SerializeObject(model, _serializerSettings);
            request.Content = new StringContent(body);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(System.Net.Mime.MediaTypeNames.Application.Json);
            return request;
        }

        private async Task<UserResearchClientResponse> SendRequestAndParseResponse(HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            return new UserResearchClientResponse(responseMessage.StatusCode);
        }
    }
}
