using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Client
{
    internal sealed class EmisClientRequestSender
    {
        private readonly ILogger<EmisClientRequestSender> _logger;
        private readonly EmisHttpClient _httpClient;
        private readonly IJsonResponseParser _responseParser;

        public EmisClientRequestSender(
            ILogger<EmisClientRequestSender> logger,
            EmisHttpClient httpClient,
            IJsonResponseParser responseParser)
        {
            _logger = logger;
            _httpClient = httpClient;
            _responseParser = responseParser;
        }

        internal async Task<EmisApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            Func<IEmisRequestBuilderType, IEmisRequestBuilder> configure)
        {
            using var request = new EmisRequest();

            configure(request);

            var responseMessage = await _httpClient.Client.SendAsync(request.RequestMessage);

            if (await IsUnauthorisedResponse(responseMessage))
            {
                _logger.LogInformation("Unauthorised EMIS response");
                throw new UnauthorisedGpSystemHttpRequestException();
            }

            var response = new EmisApiObjectResponse<TResponse>(responseMessage.StatusCode, request.Type, request.SuccessStatusCodes);
            await response.Parse(responseMessage, _responseParser, _logger);
            return response;
        }

        private async Task<bool> IsUnauthorisedResponse(HttpResponseMessage response)
        {
            return response. StatusCode == HttpStatusCode.Unauthorized ||
                string.Equals(await response.StringResponse(_logger), EmisApiErrorMessages.EmisService_UnauthorisedRequest, StringComparison.Ordinal);
        }
    }
}