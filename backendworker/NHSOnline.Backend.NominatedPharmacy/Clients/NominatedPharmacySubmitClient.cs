using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;

namespace NHSOnline.Backend.NominatedPharmacy.Clients
{
    public class NominatedPharmacySubmitClient : INominatedPharmacySubmitClient
    {
        private const string HeaderSoapAction = "SoapAction";
        private const string PdsPath = "sync-service";

        private readonly NominatedPharmacyHttpClient _httpClient;
        private readonly ILogger<NominatedPharmacySubmitClient> _logger;
        private readonly IXmlResponseParser _responseParser;

        public NominatedPharmacySubmitClient(NominatedPharmacyHttpClient httpClient,
            ILogger<NominatedPharmacySubmitClient> logger, IXmlResponseParser responseParser)
        {
            _logger = logger;
            _httpClient = httpClient;
            _responseParser = responseParser;
        }

        public async Task<UpdateNominatedPharmacyApiObjectResponse> UpdateNominatedPharmacy(
            NominatedPharmacyUpdateRequest nominatedPharmacyUpdateRequest)
        {
            try
            {
                _logger.LogEnter();

                var content = BuildContent(nominatedPharmacyUpdateRequest);

                var httpRequest = BuildHttpRequest(PdsPath, content);

                return await SendRequestAndParseResponse(httpRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred when updating nominated pharmacy");
                throw;
            }
        }

        private static HttpRequestMessage BuildHttpRequest(string path, StringContent content)
        {
            var httpRequest =
                new HttpRequestMessage(HttpMethod.Post, path)
                {
                    Content = content
                };

            httpRequest.Headers.Add(
                HeaderSoapAction,
                new[]
                {
                    "urn:nhs:names:services:pds/PRPA_IN000203UK03"
                });

            return httpRequest;
        }

        private static StringContent BuildContent(NominatedPharmacyUpdateRequest nominatedPharmacyUpdateRequest)
        {
            const string contentType = "Content-Type";
            var content = new StringContent(nominatedPharmacyUpdateRequest.Body(), Encoding.UTF8);

            content.Headers.Remove(contentType);
            content.Headers.TryAddWithoutValidation(contentType,
                new[]
                {
                    "multipart/related; boundary=\"--=_MIME-Boundary\"; type=\"text/xml\"; start=\"<ebXMLHeader@spine.nhs.uk>\";"
                });

            return content;
        }

        private async Task<UpdateNominatedPharmacyApiObjectResponse> SendRequestAndParseResponse(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);

            var response = new UpdateNominatedPharmacyApiObjectResponse(responseMessage.StatusCode);
            await response.Parse(responseMessage, _responseParser, _logger);

            return response;
        }
    }
}