using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;
using NHSOnline.Backend.Support.ResponseParsers;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.GetNominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.Clients
{
    public class NominatedPharmacyPDSClient : INominatedPharmacyPDSClient
    {
        public const string MediaType = "text/xml";

        private const string HeaderSoapAction = "SoapAction";

        private readonly NominatedPharmacyHttpClient _httpClient;
        private readonly ILogger<NominatedPharmacyPDSClient> _logger;
        private readonly IXmlResponseParser _responseParser;
        private readonly INominatedPharmacyEnvelopeService _envelopeService;
        private readonly INominatedPharmacyConfigurationSettings _settings;

        public NominatedPharmacyPDSClient(NominatedPharmacyHttpClient httpClient,
            ILogger<NominatedPharmacyPDSClient> logger, IXmlResponseParser responseParser,
            INominatedPharmacyEnvelopeService envelopeService, INominatedPharmacyConfigurationSettings settings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _responseParser = responseParser;
            _envelopeService = envelopeService;
            _settings = settings;
        }

        public async Task<NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>> NominatedPharmacyGet(
            QUPAIN000008UK02 getNominatedPharmacyRequest)
        {
            var serviceDefinition = new GetNominatedPharmacyServiceDefinition();

            var path = string.Format(CultureInfo.InvariantCulture, _settings.PdsPath);

            var result = await Post<QUPAIN000009UK03Response, QUPAIN000008UK02>(
                serviceDefinition,
                path,
                getNominatedPharmacyRequest);

            return result;
        }

        private async Task<NominatedPharmacyApiObjectResponse<TResponse>> Post<TResponse, TRequest>(
            IServiceDefinition serviceDefinition, string path, TRequest request)
        {
            using (var httpRequest = BuildHttpRequest(serviceDefinition, path, request))
            {
                return await SendRequestAndParseResponse<TResponse>(httpRequest);
            }
        }

        private HttpRequestMessage BuildHttpRequest<T>(IServiceDefinition serviceDefinition, string path, T request)
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
    }
}