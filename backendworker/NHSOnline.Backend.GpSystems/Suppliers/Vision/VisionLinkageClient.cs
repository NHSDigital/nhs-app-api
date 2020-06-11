using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionLinkageClient : IVisionLinkageClient
    {
        private readonly VisionLinkageHttpClient _httpClient;
        private readonly ILogger<VisionLinkageClient> _logger;
        private readonly IJsonResponseParser _responseParser;

        private const string LinkageBasePath = "organisations/{0}/onlineservices/linkage";

        private readonly string GetLinkagePath = LinkageBasePath + "?nhsNumber={1}";

        public VisionLinkageClient(
            ILogger<VisionLinkageClient> logger,
            VisionLinkageHttpClient httpClient,
            IJsonResponseParser responseParser)
        {
            _logger = logger;
            _httpClient = httpClient;
            _responseParser = responseParser;
        }

        public async Task<VisionLinkageApiObjectResponse<LinkageKeyGetResponse>> GetLinkageKey(GetLinkageKey getLinkageKey)
        {
            var path = string.Format(CultureInfo.InvariantCulture, GetLinkagePath, getLinkageKey.OdsCode, getLinkageKey.NhsNumber);

            return await Get<LinkageKeyGetResponse>(path);
        }

        public async Task<VisionLinkageApiObjectResponse<LinkageKeyPostResponse>> CreateLinkageKey(CreateLinkageKey createLinkageKey)
        {
            var path = string.Format(CultureInfo.InvariantCulture, LinkageBasePath, createLinkageKey.OdsCode);

            return await Post<LinkageKeyPostResponse>(createLinkageKey.LinkageKeyPostRequest, path);
        }

        private async Task<VisionLinkageApiObjectResponse<TResponse>> Get<TResponse>(string path)
        {
            using (var request = BuildVisionRequest(HttpMethod.Get, path))
            {
                return await SendRequestAndParseResponse<TResponse>(request);
            }
        }

        private async Task<VisionLinkageApiObjectResponse<TResponse>> Post<TResponse>(LinkageKeyPostRequest model, string path)
        {
            using (var request = BuildVisionRequest(HttpMethod.Post, path))
            {
                var body = JsonConvert.SerializeObject(model,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                return await SendRequestAndParseResponse<TResponse>(request);
            }
        }

        private static HttpRequestMessage BuildVisionRequest(HttpMethod httpMethod, string path)
        {
            var request = new HttpRequestMessage(httpMethod, path);

            return request;
        }

        private async Task<VisionLinkageApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            _logger.LogEnter();

            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new VisionLinkageApiObjectResponse<TResponse>(responseMessage.StatusCode);

            await response.Parse(responseMessage, _responseParser, _logger);

            _logger.LogExit();

            return response;
        }
    }
}