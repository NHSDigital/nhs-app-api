using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class OrganDonationResponse<TBody> : ApiResponse
    {
        public OrganDonationResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }
        
        public DateTimeOffset? Expires { get; set; }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

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

        public string ErrorForLogging => $"Status Code: '{StatusCode}'. {ErrorResponse?.Issue?.FirstOrDefault()}";

        private OrganDonationResponse<TBody> ParseResponse(
            IResponseParser responseParser,
            ILogger logger,
            string stringResponse,
            HttpResponseMessage responseMessage)
        {
            if (!HasSuccessResponse)
            {
                ErrorResponse = responseParser.ParseBody<OrganDonationErrorResponse>(
                    stringResponse);
                logger.LogError($"Server returned with error. {ErrorForLogging}");
                return this;
            }

            Expires = responseMessage.Content?.Headers?.Expires;

            Body = responseParser.ParseBody<TBody>(stringResponse);
            return this;
        }

        protected override bool FormatResponseIfUnsuccessful => true;
    }
}