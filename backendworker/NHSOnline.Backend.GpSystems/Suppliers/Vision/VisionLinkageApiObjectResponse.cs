using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public sealed class VisionLinkageApiObjectResponse<TBody> : ApiResponse
    {
        public VisionLinkageApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public TBody Body { get; set; }

        public ErrorResponse ErrorResponse { get; set; }

        public async Task Parse(
            HttpResponseMessage responseMessage,
            IJsonResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);

            if (!string.IsNullOrEmpty(stringResponse))
            {
                ParseResponse(responseParser, stringResponse);
            }
        }

        private void ParseResponse(
            IResponseParser responseParser,
            string stringResponse)
        {
            Body = responseParser.ParseBody<TBody>(stringResponse);

            if (!HasSuccessResponse)
            {
                var errorWrapper = responseParser.ParseBody<ErrorResponseWrapper>(stringResponse);
                ErrorResponse = errorWrapper?.Error;
            }
        }

        public override bool HasSuccessResponse => ErrorResponse == null && StatusCode.IsSuccessStatusCode();

        protected override bool FormatResponseIfUnsuccessful => true;

        private sealed class ErrorResponseWrapper
        {
            public ErrorResponse Error { get; set; }
        }
    }
}