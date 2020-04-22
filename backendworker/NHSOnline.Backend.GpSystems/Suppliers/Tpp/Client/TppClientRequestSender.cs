using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestSender: ITppClientRequestSender
    {
        private readonly TppHttpClient _httpClient;
        private readonly IXmlResponseParser _responseParser;
        private readonly ILogger<TppClientRequestSender> _logger;

        public TppClientRequestSender(
            TppHttpClient httpClient,
            IXmlResponseParser responseParser,
            ILogger<TppClientRequestSender> logger)
        {
            _httpClient = httpClient;
            _responseParser = responseParser;
            _logger = logger;
        }

        public async Task<TppApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            using (var responseMessage = await _httpClient.Client.SendAsync(request))
            {
                var response = new TppApiObjectResponse<TResponse>(responseMessage.StatusCode);
                await response.Parse(responseMessage, _responseParser, _logger);

                if (!response.IsUnauthorisedResponse)
                {
                    return response;
                }

                _logger.LogInformation("Unauthorised TPP response");
                throw new UnauthorisedGpSystemHttpRequestException();
            }
        }
    }
}