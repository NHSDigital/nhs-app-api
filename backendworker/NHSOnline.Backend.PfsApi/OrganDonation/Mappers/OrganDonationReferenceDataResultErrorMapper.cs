using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class
        OrganDonationReferenceDataResultErrorMapper : OrganDonationErrorResponseMapper<OrganDonationReferenceDataResult>
    {
        public OrganDonationReferenceDataResultErrorMapper(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Tuple<OrganDonationReferenceDataResult, string> DefaultResult =>
            new Tuple<OrganDonationReferenceDataResult, string>(
                new OrganDonationReferenceDataResult.UpstreamError
                (
                    new ApiErrorResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "A non-recoverable exception has occurred processing the request"
                    }
                ), "Something went wrong when retrieving organ donation record"
            );

        protected override Dictionary<HttpStatusCode, Tuple<OrganDonationReferenceDataResult, string>> MappingTable
            => new Dictionary<HttpStatusCode, Tuple<OrganDonationReferenceDataResult, string>>
            {
                {
                    HttpStatusCode.RequestTimeout, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.Timeout(),
                        "The organ donation registration timed-out")
                },
                {
                    HttpStatusCode.InternalServerError, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.UpstreamError(NoRetryResponse), 
                        "The organ donation request is InternalServerError")
                },
                {
                    HttpStatusCode.TooManyRequests, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.UpstreamError(RetryResponse),
                        "The organ donation request is TooManyRequests")
                },
                {
                    HttpStatusCode.ServiceUnavailable, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.UpstreamError(RetryResponse),
                        "The organ donation request is ServiceUnavailable")
                },
                {
                    HttpStatusCode.GatewayTimeout, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.UpstreamError(RetryResponse),
                        "The organ donation request is GatewayTimeout")
                },
                {
                    HttpStatusCode.BadGateway, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.UpstreamError(NoRetryResponse),
                        "The organ donation request is BadGateway")
                },
                {
                    HttpStatusCode.BadRequest, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.SystemError(),
                        "The organ donation request is BadRequest")
                },
                {
                    HttpStatusCode.Unauthorized, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.SystemError(),
                        "The organ donation request is Unauthorized")
                },
                {
                    HttpStatusCode.Forbidden, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.SystemError(),
                        "The organ donation request is Forbidden")
                },
                {
                    HttpStatusCode.MethodNotAllowed, new Tuple<OrganDonationReferenceDataResult, string>
                    (new OrganDonationReferenceDataResult.SystemError(),
                        "The organ donation request is MethodNotAllowed")
                }
            };
    }
}