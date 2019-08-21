using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class
        OrganDonationRegistrationResultErrorMapper : OrganDonationErrorResponseMapper<OrganDonationRegistrationResult>
    {
        public OrganDonationRegistrationResultErrorMapper(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Tuple<OrganDonationRegistrationResult, string> DefaultResult =>
            new Tuple<OrganDonationRegistrationResult, string>(
                new OrganDonationRegistrationResult.UpstreamError
                (
                    new PfsErrorResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "A non-recoverable exception has occurred processing the request"
                    }
                ),
                "Something went wrong when registering organ donation decision"
            );

        protected override Dictionary<HttpStatusCode, Tuple<OrganDonationRegistrationResult, string>> MappingTable
            => new Dictionary<HttpStatusCode, Tuple<OrganDonationRegistrationResult, string>>
            {
                {
                    HttpStatusCode.RequestTimeout, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.Timeout(),
                        "The organ donation registration timed-out")
                },
                {
                    HttpStatusCode.InternalServerError, new Tuple<OrganDonationRegistrationResult, string>
                    (new OrganDonationRegistrationResult.UpstreamError(NoRetryResponse),
                        "The organ donation request is InternalServerError")
                },
                {
                    HttpStatusCode.TooManyRequests, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.UpstreamError(RetryResponse),
                        "The organ donation request is TooManyRequests")
                },
                {
                    HttpStatusCode.ServiceUnavailable, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.UpstreamError(RetryResponse),
                        "The organ donation request is ServiceUnavailable")
                },
                {
                    HttpStatusCode.GatewayTimeout, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.UpstreamError(RetryResponse),
                        "The organ donation request is GatewayTimeout")
                },
                {
                    HttpStatusCode.BadGateway, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.UpstreamError(NoRetryResponse),
                        "The organ donation request is BadGateway")
                },
                {
                    HttpStatusCode.NotFound, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.UpstreamError(NoRetryResponse),
                        "The organ donation request is NotFound")
                },
                {
                    HttpStatusCode.BadRequest, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.SystemError(),
                        "The organ donation request is BadRequest")
                },
                {
                    HttpStatusCode.Unauthorized, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.SystemError(),
                        "The organ donation request is Unauthorized")
                },
                {
                    HttpStatusCode.Forbidden, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.SystemError(),
                        "The organ donation request is Forbidden")
                },
                {
                    HttpStatusCode.MethodNotAllowed, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.SystemError(),
                        "The organ donation request is MethodNotAllowed")
                },
                {
                    HttpStatusCode.UnsupportedMediaType, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.SystemError(),
                        "The organ donation request is UnsupportedMediaType")
                },
                {
                    HttpStatusCode.UnprocessableEntity, new Tuple<OrganDonationRegistrationResult, string>(
                        new OrganDonationRegistrationResult.SystemError(),
                        "The organ donation request is UnprocessableEntity")
                },
            };
    }
}