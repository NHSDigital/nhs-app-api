using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public class NhsApimAuthResponse<TBody> : ApiResponse
    {
        public NhsApimAuthResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        protected override bool FormatResponseIfUnsuccessful => false;

        public TBody Body { get; private set; }

        public async Task<NhsApimAuthResponse<TBody>> Parse(
            HttpResponseMessage responseMessage,
            IJsonResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(
                responseMessage,
                logger,
                "Auth request for APIM Aggregator");

            return string.IsNullOrEmpty(stringResponse)
                ? this
                : ParseResponse(responseParser, stringResponse);
        }

        private NhsApimAuthResponse<TBody> ParseResponse(IJsonResponseParser responseParser, string stringResponse)
        {
            if (HasSuccessResponse)
            {
                Body = responseParser.ParseBody<TBody>(stringResponse);
            }

            return this;
        }
    }
}