using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationResultErrorMapper : OrganDonationErrorResponseMapper<OrganDonationResult>
    {
        public OrganDonationResultErrorMapper(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Tuple<OrganDonationResult, string> DefaultResult => new Tuple<OrganDonationResult, string>(
            new OrganDonationResult.SearchError(),
            "Something went wrong when retrieving organ donation record"
        );

        protected override Dictionary<HttpStatusCode, Tuple<OrganDonationResult, string>> MappingTable
            => new Dictionary<HttpStatusCode, Tuple<OrganDonationResult, string>>
            {
                {
                    HttpStatusCode.BadRequest, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchError(),
                        "The organ donation request is invalid")
                },
                {
                    HttpStatusCode.Unauthorized, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchError(),
                        "The organ donation request is Unauthorized")
                },
                {
                    HttpStatusCode.Forbidden, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchError(),
                        "The organ donation request is Forbidden")
                },
                {
                    HttpStatusCode.RequestTimeout, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchTimeout(), 
                        "The organ donation request is RequestTimeout")
                },
                {
                    HttpStatusCode.InternalServerError, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchUpstreamError(NoRetryResponse),
                        "The organ donation request is InternalServerError")
                },
                {
                    HttpStatusCode.TooManyRequests, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchUpstreamError(RetryResponse),
                        "The organ donation request is TooManyRequests")
                },
                {
                    HttpStatusCode.ServiceUnavailable, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchUpstreamError(RetryResponse),
                        "The organ donation request is ServiceUnavailable")
                },
                {
                    HttpStatusCode.GatewayTimeout, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchUpstreamError(RetryResponse),
                        "The organ donation request is GatewayTimeout")
                },
                {
                    HttpStatusCode.MethodNotAllowed, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchError(),
                        "The organ donation request is MethodNotAllowed")
                },
                {
                    HttpStatusCode.UnsupportedMediaType, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchError(),
                        "The organ donation request is UnsupportedMediaType")
                },
                {
                    HttpStatusCode.UnprocessableEntity, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchError(),
                        "The organ donation request is UnprocessableEntity")
                },
                {
                    HttpStatusCode.BadGateway, new Tuple<OrganDonationResult, string>(
                        new OrganDonationResult.SearchUpstreamError(NoRetryResponse),
                        "The organ donation request is BadGateway")
                },
            };
    }
}