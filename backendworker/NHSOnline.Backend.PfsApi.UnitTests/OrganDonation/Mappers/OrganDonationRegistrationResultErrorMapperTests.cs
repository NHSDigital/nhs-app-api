using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class OrganDonationRegistrationResultErrorMapperTests
    {
        private IMapper<HttpStatusCode, OrganDonationRegistrationResult> _organDonationRegistrationResultErrorMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationRegistrationResultErrorMapper = fixture.Create<OrganDonationRegistrationResultErrorMapper>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        [DataRow(HttpStatusCode.GatewayTimeout)]
        public void MapOrganDonationAPIError_WhenRecoverableErrorHttpStatus_MapsToUpstreamErrorWithRetry(HttpStatusCode httpStatus)
        {
            // Arrange
            var response = new ApiErrorResponse
            {
                ErrorCode = 1,
                ErrorMessage = "A recoverable exception has occurred processing the request"
            };
            var expectedErrorResult = new OrganDonationRegistrationResult.UpstreamError(response);

            // Act
            var result = _organDonationRegistrationResultErrorMapper.Map(httpStatus);

            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
        }
        
        [TestMethod]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        public void MapOrganDonationAPIError_WhenNonRecoverableErrorHttpStatus_MapsToUpstreamErrorWithoutRetry(HttpStatusCode httpStatus)
        {
            // Arrange
            var response = new ApiErrorResponse
            {
                ErrorCode = 0,
                ErrorMessage = "A non-recoverable exception has occurred processing the request"
            };
            
            var expectedErrorResult = new OrganDonationRegistrationResult.UpstreamError(response);

            // Act
            var result = _organDonationRegistrationResultErrorMapper.Map(httpStatus);

            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
        }
        
        [TestMethod]
        [DataRow(HttpStatusCode.RequestTimeout)]
        public void MapOrganDonationAPIError_WhenPassingRequestTimeoutHttpStatus_MapsToTimeoutError(HttpStatusCode httpStatus)
        {
            // Act
            var result = _organDonationRegistrationResultErrorMapper.Map(httpStatus);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrganDonationRegistrationResult.Timeout>();
        }
        
        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.MethodNotAllowed)]
        [DataRow(HttpStatusCode.UnsupportedMediaType)]
        [DataRow(HttpStatusCode.UnprocessableEntity)]
        public void MapOrganDonationAPIError_WhenPassingRequestErrorHttpStatus_MapsToSystemError(HttpStatusCode httpStatus)
        { 
            // Act
            var result = _organDonationRegistrationResultErrorMapper.Map(httpStatus);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrganDonationRegistrationResult.SystemError>();
        }
    }
}