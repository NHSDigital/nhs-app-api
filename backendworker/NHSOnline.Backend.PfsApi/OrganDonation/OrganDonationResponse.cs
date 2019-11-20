using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class OrganDonationResponse<TBody> : ApiResponse
    {
        public OrganDonationResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }
        
        public DateTimeOffset? Expires { get; set; }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        public override bool HasBadRequestResponse => StatusCode.IsBadRequestCode();
        public OrganDonationErrorResponse ErrorResponse { get; private set; }

        public TBody Body { get; set; }

        public async Task<OrganDonationResponse<TBody>> Parse(
            HttpResponseMessage responseMessage,
            IJsonResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);
            return string.IsNullOrEmpty(stringResponse)
                ? this
                : ParseResponse(responseParser, logger, stringResponse, responseMessage);
        }

        public override string ErrorForLogging => $"Status Code: '{StatusCode}'. {ErrorResponse?.Issue?.FirstOrDefault()}";

        private OrganDonationResponse<TBody> ParseResponse(
            IResponseParser responseParser,
            ILogger logger,
            string stringResponse,
            HttpResponseMessage responseMessage)
        {
            if (!HasSuccessResponse)
            {
                var parseErrorSuccess = responseParser.TryParseBody<OrganDonationErrorResponse>(
                    stringResponse, 
                    responseMessage, 
                    out var response);
                if (!parseErrorSuccess)
                {
                    logger.LogError("An error occured while parsing the response");
                }
                logger.LogError($"Server returned with error. {ErrorForLogging}");
                ErrorResponse = response;
                return this;
            }

            Expires = responseMessage.Content?.Headers?.Expires;

            var parseSuccess = responseParser.TryParseBody<TBody>(stringResponse, responseMessage, out var body);
            if (!parseSuccess)
            {
                logger.LogError("An error occured while parsing the response");
            }

            Body = body;
            return this;
        }

        protected override bool FormatResponseIfUnsuccessful => true;
    }
}