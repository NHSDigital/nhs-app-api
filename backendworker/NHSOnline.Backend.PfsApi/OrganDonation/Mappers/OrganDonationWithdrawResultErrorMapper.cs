using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class
        OrganDonationWithdrawResultErrorMapper : OrganDonationErrorResponseMapper<OrganDonationWithdrawResult>
    {
        public OrganDonationWithdrawResultErrorMapper(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Tuple<OrganDonationWithdrawResult, string> DefaultResult =>
            new Tuple<OrganDonationWithdrawResult, string>(
                new OrganDonationWithdrawResult.UpstreamError
                (
                    new PfsErrorResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "A non-recoverable exception has occurred processing the request"
                    }
                ),
                "Something went wrong when withdrawing organ donation decision"
            );



        protected override Dictionary<HttpStatusCode, Tuple<OrganDonationWithdrawResult, string>> MappingTable
            => new Dictionary<HttpStatusCode, Tuple<OrganDonationWithdrawResult, string>>
            {
                {
                    HttpStatusCode.RequestTimeout, new Tuple<OrganDonationWithdrawResult, string>(
                        new OrganDonationWithdrawResult.Timeout(),
                        "The organ donation withdraw timed-out")
                },
                {
                    HttpStatusCode.InternalServerError,
                    UpstreamError(NoRetryResponse, HttpStatusCode.InternalServerError)
                },
                {
                    HttpStatusCode.BadGateway, UpstreamError(NoRetryResponse, HttpStatusCode.BadGateway)
                },
                {
                    HttpStatusCode.TooManyRequests, UpstreamError(RetryResponse, HttpStatusCode.TooManyRequests)
                },
                {
                    HttpStatusCode.ServiceUnavailable, UpstreamError(RetryResponse, HttpStatusCode.ServiceUnavailable)
                },
                {
                    HttpStatusCode.GatewayTimeout, UpstreamError(RetryResponse, HttpStatusCode.GatewayTimeout)
                },
                {
                    HttpStatusCode.BadRequest, SystemError(HttpStatusCode.BadRequest)
                },
                {
                    HttpStatusCode.Unauthorized, SystemError(HttpStatusCode.Unauthorized)
                },
                {
                    HttpStatusCode.Forbidden, SystemError(HttpStatusCode.Forbidden)
                },
                {
                    HttpStatusCode.MethodNotAllowed, SystemError(HttpStatusCode.MethodNotAllowed)
                },
                {
                    HttpStatusCode.UnsupportedMediaType, SystemError(HttpStatusCode.UnsupportedMediaType)
                },
                {
                    HttpStatusCode.UnprocessableEntity, SystemError(HttpStatusCode.UnprocessableEntity)
                }
            };

        private static Tuple<OrganDonationWithdrawResult, string> UpstreamError(IApiErrorResponse response,
            HttpStatusCode code)
        {
            return new Tuple<OrganDonationWithdrawResult, string>(
                new OrganDonationWithdrawResult.UpstreamError(response),
                $"The organ donation withdraw request returned status code {code}");
        }


        private static Tuple<OrganDonationWithdrawResult, string> SystemError(HttpStatusCode code)
        {
            return new Tuple<OrganDonationWithdrawResult, string>(
                new OrganDonationWithdrawResult.SystemError(),
                $"The organ donation withdraw request returned status code {code}");
        }
    }
}