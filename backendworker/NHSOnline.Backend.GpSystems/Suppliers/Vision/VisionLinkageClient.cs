using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.Support;
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

        public async Task<VisionApiObjectResponse<LinkageKeyGetResponse>> GetLinkageKey(GetLinkageKey getLinkageKey)
        {
            var path = string.Format(CultureInfo.InvariantCulture, GetLinkagePath, getLinkageKey.OdsCode, getLinkageKey.NhsNumber);

            return await Get<LinkageKeyGetResponse>(path);
        }

        public async Task<VisionApiObjectResponse<LinkageKeyPostResponse>> CreateLinkageKey(CreateLinkageKey createLinkageKey)
        {
            var path = string.Format(CultureInfo.InvariantCulture, LinkageBasePath, createLinkageKey.OdsCode);
            
            return await Post<LinkageKeyPostResponse>(createLinkageKey.LinkageKeyPostRequest, path);
            
        }

        private async Task<VisionApiObjectResponse<TResponse>> Get<TResponse>(string path)
        {
            var request = BuildVisionRequest(HttpMethod.Get, path);
            return await SendRequestAndParseResponse<TResponse>(request);
        }
        
        private async Task<VisionApiObjectResponse<TResponse>> Post<TResponse>(LinkageKeyPostRequest model, string path)
        {
            var request = BuildVisionRequest(HttpMethod.Post, path);

            var body = JsonConvert.SerializeObject(model, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private static HttpRequestMessage BuildVisionRequest(HttpMethod httpMethod, string path)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            
            return request;
        }

        private async Task<VisionApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            _logger.LogEnter();

            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new VisionApiObjectResponse<TResponse>(responseMessage.StatusCode);

            await response.Parse(responseMessage, _responseParser, _logger);

            _logger.LogExit();

            return response;
        }

        public class VisionApiObjectResponse<TBody> : ApiResponse
        {
            public VisionApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public TBody Body { get; set; }

            public ErrorResponse ErrorResponse { get; set; }

            public bool HasInternalErrorCode(string code)
            {
                return string.Equals(ErrorResponse?.Code, code, StringComparison.Ordinal);
            }

            public bool HasStatusCodeAndErrorCode(HttpStatusCode statusCode, string visionApiErrorCode)
            {
                return (StatusCode == statusCode) && HasInternalErrorCode(visionApiErrorCode);
            }

            public async Task<VisionApiObjectResponse<TBody>> Parse(
                HttpResponseMessage responseMessage,
                IJsonResponseParser responseParser,
                ILogger logger)
            {
                var stringResponse = await GetStringResponse(responseMessage, logger);
                return string.IsNullOrEmpty(stringResponse)
                    ? this : ParseResponse(responseParser, stringResponse, responseMessage);
            }

            private VisionApiObjectResponse<TBody> ParseResponse(
                IResponseParser responseParser,
                string stringResponse,
                HttpResponseMessage responseMessage)
            {
                Body = responseParser.ParseBody<TBody>(stringResponse, responseMessage);
                
                if (!HasSuccessResponse)
                {
                    var errorResponseWrapper = responseParser.ParseBody<ErrorResponseWrapper>(stringResponse, responseMessage);
                    ErrorResponse = errorResponseWrapper?.Error;
                }

                return this;
            }

            public override bool HasSuccessResponse => ErrorResponse == null && StatusCode.IsSuccessStatusCode();

            public override string ErrorForLogging => ErrorResponse == null ? null : JsonConvert.SerializeObject(ErrorResponse);

            protected override bool FormatResponseIfUnsuccessful => true;
        }

        public class ErrorResponseWrapper
        {
            public ErrorResponse Error { get; set; }
        }
    }
}