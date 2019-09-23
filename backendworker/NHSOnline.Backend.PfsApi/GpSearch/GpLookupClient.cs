using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public class GpLookupClient: IGpLookupClient
    {
        private readonly ILogger<GpLookupClient> _logger;
        private readonly GpLookupHttpClient _gpLookupHttpClient;
        private readonly IJsonResponseParser _responseParser;
        private readonly IGpLookupConfig _config;
        
        private const string SubscriptionKey = "subscription-key";
        private const string OrganisationSearchPath = "service-search/search?api-version=1";
        private const string PostcodeSearchPath = "service-search/postcodesandplaces/search?api-version=1";
        
        public GpLookupClient(
            ILogger<GpLookupClient> logger, 
            GpLookupHttpClient gpLookupHttpClient, 
            IJsonResponseParser responseParser,
            IGpLookupConfig config)
        {
            _logger = logger;
            _gpLookupHttpClient = gpLookupHttpClient;
            _responseParser = responseParser;
            _config = config;
        }

        public async Task<NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>> GpSearch(OrganisationSearchData searchData)
        {
            return await Post<OrganisationSearchData, NhsOrganisationSearchResponse>(searchData,
                OrganisationSearchPath);            
        }
        
        public async Task<NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>> PostcodeSearch(PostcodeSearchData searchData)
        {
            return await Post<PostcodeSearchData, NhsPostcodeSearchResponse>(searchData,
                PostcodeSearchPath);            
        }

        public async Task<NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>>
            GpPostcodeSearch(OrganisationPostcodeSearchData searchData)
        {
            return await Post<OrganisationPostcodeSearchData, NhsOrganisationSearchResponse>(searchData,
                OrganisationSearchPath);
        }

        public async Task<NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>> PharmacySearch(OrganisationSearchData searchData)
        {
            _logger.LogEnter();
            return await Post<OrganisationSearchData, NhsOrganisationSearchResponse>(searchData,
                OrganisationSearchPath);
        }

        public async Task<NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>> PharmaciesSearch(OrganisationSearchData searchData)
        {
            _logger.LogEnter();
            return await Post<OrganisationSearchData, NhsOrganisationSearchResponse>(searchData,
                OrganisationSearchPath);
        }
        
        private async Task<NhsSearchApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model, string path)
        {
            var request = BuildNhsSearchRequest(HttpMethod.Post, path, _config.GpLookupApiKey);
            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request);
        }
        
        private static HttpRequestMessage BuildNhsSearchRequest(HttpMethod httpMethod, string path, string apiKey)
        {
            var request = new HttpRequestMessage(httpMethod, path);               
            request.Headers.Add(SubscriptionKey, apiKey);
            return request;
        }
        
        private async Task<NhsSearchApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _gpLookupHttpClient.Client.SendAsync(request);
            var response = new NhsSearchApiObjectResponse<TResponse>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }
        
        public abstract class NhsSearchApiResponse: ApiResponse
        {
            protected NhsSearchApiResponse(HttpStatusCode statusCode) :base(statusCode)
            {}

            public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

            public override bool HasBadRequestResponse => StatusCode.IsBadRequestCode();
            
            public override string  ErrorForLogging => $"Error Code: '{StatusCode}'. ";

        }
        
        public class NhsSearchApiObjectResponse<TBody> : NhsSearchApiResponse
        {
            public TBody Body { get; set; }
            public NhsSearchApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {}

            public async Task<NhsSearchApiObjectResponse<TBody>> Parse(
                HttpResponseMessage responseMessage,
                IJsonResponseParser responseParser,
                ILogger logger)
            {
                var stringResponse = await GetStringResponse(responseMessage, logger);
                return string.IsNullOrEmpty(stringResponse)
                    ? this : ParseResponse(responseParser, stringResponse, responseMessage);
            }

            private NhsSearchApiObjectResponse<TBody> ParseResponse(
                IResponseParser responseParser, 
                string stringResponse, 
                HttpResponseMessage responseMessage)
            {
                Body = responseParser.ParseBody<TBody>(stringResponse, responseMessage);
                return this;
            }

            protected override bool FormatResponseIfUnsuccessful => true;
        }
    }
}