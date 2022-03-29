using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareResponse<TBody> : ApiResponse
    {
        public SecondaryCareResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        protected override bool FormatResponseIfUnsuccessful => false;

        public TBody Body { get; private set; }

        public async Task<SecondaryCareResponse<TBody>> Parse(
            HttpResponseMessage responseMessage,
            IJsonResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);

            return string.IsNullOrEmpty(stringResponse)
                ? this
                : ParseResponse(responseParser, logger, stringResponse);
        }

        private SecondaryCareResponse<TBody> ParseResponse(IJsonResponseParser responseParser, ILogger logger, string stringResponse)
        {
            if (!HasSuccessResponse)
            {
                logger.LogError("Aggregator returned with error.");

                return this;
            }

            Body = responseParser.ParseBody<TBody>(stringResponse);

            return this;
        }
    }
}