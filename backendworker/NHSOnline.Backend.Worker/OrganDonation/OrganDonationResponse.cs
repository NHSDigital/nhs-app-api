using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.ResponseParsers;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationResponse<TBody> : ApiResponse
    {
        public OrganDonationResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

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

        public override string ErrorForLogging => $"Status Code: '{StatusCode}'. " +
                                                  $"Error Code: '{ErrorResponse?.Issue?.Code}'. " +
                                                  $"Details:'{ErrorResponse?.Issue?.Details}'. " +
                                                  $"Diagnostics:'{ErrorResponse?.Issue?.Diagnostics}'.";

        private OrganDonationResponse<TBody> ParseResponse(
            IResponseParser responseParser,
            ILogger logger,
            string stringResponse,
            HttpResponseMessage responseMessage)
        {
            if (!HasSuccessResponse)
            {
                ErrorResponse = responseParser.ParseBody<OrganDonationErrorResponse>(stringResponse, responseMessage);
                logger.LogError($"Server returned with error. {ErrorForLogging}");
                return this;
            }

            Body = responseParser.ParseBody<TBody>(stringResponse, responseMessage);
            return this;
        }

        protected override bool FormatResponseIfUnsuccessful => true;
    }
}