using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppApiObjectResponse<TBody> : TppApiResponse
    {
        private const string ResponseSuidHeader = "suid";

        public TppApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

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

        public TBody Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        internal string ErrorForLogging => $"Error Code: '{ErrorResponse?.ErrorCode}'. " +
                                           $"Error User Message:'{ErrorResponse?.UserFriendlyMessage}'. " +
                                           $"Error Technical Response:'{ErrorResponse?.TechnicalMessage}'.";

        protected override bool FormatResponseIfUnsuccessful => false;

        private void ParseResponse(
            IResponseParser responseParser,
            ILogger logger,
            string stringResponse,
            HttpResponseMessage responseMessage)
        {
            if (IsErrorResponse(stringResponse))
            {
                ErrorResponse = responseParser.ParseBody<Error>(stringResponse);
                logger.LogError($"Server returned with error. {ErrorForLogging}");
                return;
            }

            Body = responseParser.ParseBody<TBody>(stringResponse);

            if (responseMessage.Headers.TryGetValues(ResponseSuidHeader, out var values))
            {
                Headers = new Dictionary<string, string> { { ResponseSuidHeader, values.First() } };
            }
        }
    }
}