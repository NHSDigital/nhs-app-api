using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.VisionServiceDefinition;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    internal sealed class VisionPfsClientRequestSender
    {
        private readonly VisionPFSHttpClient _httpClient;
        private readonly IXmlResponseParser _responseParser;
        private readonly Uri _targetUri;
        private readonly string _requestUsername;
        private readonly string _providerId;
        private readonly ILogger _logger;
        private readonly IEnvelopeService _envelopeService;

        private const string MediaType = "text/xml";

        public VisionPfsClientRequestSender(
            ILogger<VisionPfsClientRequestSender> logger,
            IEnvelopeService envelopeService,
            VisionPFSHttpClient httpClient,
            IXmlResponseParser responseParser,
            VisionConfigurationSettings settings)
        {
            _logger = logger;
            _envelopeService = envelopeService;
            _httpClient = httpClient;
            _responseParser = responseParser;

            _targetUri = settings.ApiUrl;
            _requestUsername = settings.RequestUsername;
            _providerId = settings.ApplicationProviderId;
        }

        internal async Task<VisionPfsApiObjectResponse<TResponse>> Post<TResponse, TRequest>(
            IVisionServiceDefinition visionServiceDefinition,
            VisionUserSession session,
            TRequest requestContent)
        {
            var request = new VisionRequest<TRequest>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                session.RosuAccountId,
                session.ApiKey,
                session.OdsCode,
                _providerId,
                requestContent);

            return await Post<TResponse, TRequest>(request);
        }

        internal async Task<VisionPfsApiObjectResponse<TResponse>> Post<TResponse, TRequest>(VisionRequest<TRequest> request)
        {
            using var httpRequest = BuildHttpRequest(request);

            return await SendRequestAndParseResponse<TResponse>(httpRequest);
        }

        private HttpRequestMessage BuildHttpRequest<T>(VisionRequest<T> request)
        {
            var envelope = _envelopeService.BuildEnvelope(request, _requestUsername);
            var httpRequest =
                new HttpRequestMessage(HttpMethod.Post, _targetUri)
                {
                    Content = new StringContent(envelope, Encoding.UTF8, MediaType)
                };

            httpRequest.Headers.Add(Constants.VisionConstants.RequestIdentifierHeader, request?.ServiceDefinition?.Name);

            return httpRequest;
        }

        private async Task<VisionPfsApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>
            (HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new VisionPfsApiObjectResponse<TResponse>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }
    }
}