using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class OrganDonationReferenceDataResultErrorMapperTests
    {
        private IMapper<HttpStatusCode, OrganDonationReferenceDataResult> _organDonationReferenceDataResultErrorMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationReferenceDataResultErrorMapper =
                fixture.Create<OrganDonationReferenceDataResultErrorMapper>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        [DataRow(HttpStatusCode.GatewayTimeout)]
        public void MapOrganDonationAPIError_WhenRecoverableErrorHttpStatus_MapsToUpstreamErrorWithRetry(
            HttpStatusCode httpStatus)
        {
            // Arrange
            var response = new PfsErrorResponse
            {
                ErrorCode = 1,
                ErrorMessage = "A recoverable exception has occurred processing the request"
            };

            var expectedErrorResult = new OrganDonationReferenceDataResult.UpstreamError(response);

            // Act
            var result = _organDonationReferenceDataResultErrorMapper.Map(httpStatus);

            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.BadGateway)]
        public void MapOrganDonationAPIError_WhenNonRecoverableErrorHttpStatus_MapsToUpstreamErrorWithoutRetry(
            HttpStatusCode httpStatus)
        {
            // Arrange
            var response = new PfsErrorResponse
            {
                ErrorCode = 0,
                ErrorMessage = "A non-recoverable exception has occurred processing the request"
            };

            var expectedErrorResult =
                new OrganDonationReferenceDataResult.UpstreamError(response);

            // Act
            var result = _organDonationReferenceDataResultErrorMapper.Map(httpStatus);

            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
        }

        [TestMethod]
        public void MapOrganDonationAPIError_WhenPassingRequestTimeoutHttpStatus_MapsToTimeoutError()
        {
            // Act
            var result = _organDonationReferenceDataResultErrorMapper.Map(HttpStatusCode.RequestTimeout);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrganDonationReferenceDataResult.Timeout>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.MethodNotAllowed)]
        public void MapOrganDonationAPIError_WhenPassingRequestErrorHttpStatus_MapsToSystemError(
            HttpStatusCode httpStatus)
        {
            // Act
            var result = _organDonationReferenceDataResultErrorMapper.Map(httpStatus);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrganDonationReferenceDataResult.SystemError>();
        }
    }
}