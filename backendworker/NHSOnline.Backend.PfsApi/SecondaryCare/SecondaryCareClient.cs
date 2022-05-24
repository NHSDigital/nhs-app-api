using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using Constants = NHSOnline.Backend.Support.Constants.SecondaryCareConstants;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareClient : ISecondaryCareClient
    {
        private const string SummaryPath = "summary/$evaluate";

        /*
         * Base 64 encoding of the Aggregators Target Identifier within the scope of BaRS (Booking and Referrals Standard)
         *
         * {
         *     "system": "urn:ietf:rfc:3986",
         *     "value": "db71698b-cd7c-4dd5-95c4-0aa9776595f5"
         * }
         *
         */
        private const string NHSDTargetIdentifier =
            "ewrCoCDCoCAic3lzdGVtIjogInVybjppZXRmOnJmYzozOTg2IiwKwqAgwqAgInZh" +
            "bHVlIjogImRiNzE2OThiLWNkN2MtNGRkNS05NWM0LTBhYTk3NzY1OTVmNSIKfQ==";

        private readonly SecondaryCareHttpClient _httpClient;
        private readonly ILogger<SecondaryCareClient> _logger;
        private readonly IGuidCreator _guidCreator;

        public SecondaryCareClient(
            SecondaryCareHttpClient httpClient,
            ILogger<SecondaryCareClient> logger,
            IGuidCreator guidCreator)
        {
            _httpClient = httpClient;
            _logger = logger;
            _guidCreator = guidCreator;
        }

        public async Task<SecondaryCareResponse> GetSummary(P9UserSession userSession, string accessToken)
            => await Get(SummaryPath, userSession.NhsNumber.RemoveWhiteSpace(), accessToken);

        private async Task<SecondaryCareResponse> Get(string path, string nhsNumber, string accessToken)
        {
            var queryBuilder = new QueryBuilder
            {
                { Constants.PatientIdentifierQuery, $"{Constants.PatientIdentifierPrefix}{nhsNumber}" }
            };
            var pathAndQuery = $"{path}{queryBuilder.ToQueryString()}";
            var correlationId = _guidCreator.CreateGuid().ToString();
            var requestId = _guidCreator.CreateGuid().ToString();

            using var request = new HttpRequestMessage(HttpMethod.Get, pathAndQuery);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add(Constants.CorrelationIdHeaderKey, correlationId);
            request.Headers.Add(Constants.RequestIdHeaderKey, requestId);
            request.Headers.Add(Constants.NHSDTargetIdentifierHeaderKey, NHSDTargetIdentifier);

            return await SendRequestAndParseResponse(request);
        }

        private async Task<SecondaryCareResponse> SendRequestAndParseResponse(HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new SecondaryCareResponse(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _logger);
        }
    }
}