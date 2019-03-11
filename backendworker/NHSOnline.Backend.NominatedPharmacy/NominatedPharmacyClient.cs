using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.NominatedPharmacy.ApiModels;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyClient : INominatedPharmacyClient
    {
        public const string MediaType = "text/xml";

        private const string HeaderSoapAction = "SoapAction";

        private const string PdsPath = "syncservice-pds/pds";

        private readonly NominatedPharmacyHttpClient _httpClient;
        private readonly ILogger<NominatedPharmacyClient> _logger;
        private readonly IXmlResponseParser _responseParser;
        private readonly IEnvelopeService _envelopeService;

        public NominatedPharmacyClient(NominatedPharmacyHttpClient httpClient, ILogger<NominatedPharmacyClient> logger, IXmlResponseParser responseParser, IEnvelopeService envelopeService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _responseParser = responseParser;
            _envelopeService = envelopeService;
        }

        public async Task<NominatedPharmacyApiObjectResponse<QUPA_IN000009UK03_Response>> NominatedPharmacyGet(QUPA_IN000008UK02 getNominatedPharmacyRequest)
        {
            var serviceDefinition = new GetNominatedPharmacyServiceDefinition();
            
            var path = string.Format(CultureInfo.InvariantCulture, PdsPath);

            var result = await Post<QUPA_IN000009UK03_Response, QUPA_IN000008UK02>(
                serviceDefinition,
                path,
                getNominatedPharmacyRequest);

            return result;
        }

        private async Task<NominatedPharmacyApiObjectResponse<TResponse>> Post<TResponse, TRequest>(IServiceDefinition serviceDefinition, string path, TRequest requestContent)
        {
            var request = new PdsRequest<TRequest>(requestContent);

            return await Post<TResponse, TRequest>(serviceDefinition, path, request);            
        }

        private async Task<NominatedPharmacyApiObjectResponse<TResponse>> Post<TResponse, TRequest>(IServiceDefinition serviceDefinition, string path, PdsRequest<TRequest> request)
        {
            var httpRequest = BuildHttpRequest(serviceDefinition, path, request);
            var response = await SendRequestAndParseResponse<TResponse>(httpRequest);

            return response;
        }
        
        private HttpRequestMessage BuildHttpRequest<T>(IServiceDefinition serviceDefinition, string path, PdsRequest<T> request)
        {
            var envelope = _envelopeService.BuildEnvelope(request, serviceDefinition);
            var httpRequest =
                new HttpRequestMessage(HttpMethod.Post, path)
                {
                    Content = new StringContent(envelope, Encoding.UTF8, MediaType)
                };
            
            httpRequest.Headers.Add(HeaderSoapAction, new[] { serviceDefinition.SoapActionName });

            return httpRequest;
        }

        private async Task<NominatedPharmacyApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            
            var response = new NominatedPharmacyApiObjectResponse<TResponse>(responseMessage.StatusCode);
            await response.Parse(responseMessage, _responseParser, _logger);
            return response;
        }

        public abstract class NominatedPharmacyApiResponse : ApiResponse
        {
            protected NominatedPharmacyApiResponse(HttpStatusCode statusCode) : base(statusCode)
            { }

            public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
            public override string ErrorForLogging => $"Error Code: '{StatusCode}'.";
        }

        public class NominatedPharmacyApiObjectResponse<TBody> : NominatedPharmacyApiResponse
        {
            public NominatedPharmacyResponseEnvelope<TBody> RawResponse { get; set; }

            public TBody Body => RawResponse.Body.RetrievalQueryResponse;

            public NominatedPharmacyApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            { }

            public async Task Parse(
                HttpResponseMessage responseMessage,
                IXmlResponseParser responseParser,
                ILogger logger)
            {
                var stringResponse = await GetStringResponse(responseMessage, logger);
                if (!string.IsNullOrEmpty(stringResponse))
                {
                    ParseResponse(responseParser, logger, stringResponse, responseMessage);
                }
            }

            private void ParseResponse(
                IResponseParser responseParser,
                ILogger logger,
                string stringResponse,
                HttpResponseMessage responseMessage)
            {
                try
                {
                    RawResponse = responseParser.ParseBody<NominatedPharmacyResponseEnvelope<TBody>>(stringResponse, responseMessage);
                }
                catch (FormatException e)
                {
                    logger.LogError(e, "An error occured while parsing the response");
                }
            }

            protected override bool FormatResponseIfUnsuccessful => false;
        }
    }
}