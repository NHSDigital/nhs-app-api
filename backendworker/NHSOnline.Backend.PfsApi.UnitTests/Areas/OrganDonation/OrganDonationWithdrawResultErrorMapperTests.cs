using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public class OrganDonationWithdrawResultErrorMapperTests
    {
        private IMapper<HttpStatusCode, OrganDonationWithdrawResult> _organDonationResultErrorMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationResultErrorMapper = fixture.Create<OrganDonationWithdrawResultErrorMapper>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        [DataRow(HttpStatusCode.GatewayTimeout)]
        public void MapOrganDonationAPIError_WhenRecoverableErrorHttpStatus_MapsToBadGatewayErrorWithRetry(HttpStatusCode httpStatus)
        {
            // Arrange         
            var response = new PfsErrorResponse
            {
                ErrorCode = 1,
                ErrorMessage = "A recoverable exception has occurred processing the request"
            };
            
            var expectedErrorResult = new OrganDonationWithdrawResult.UpstreamError(response);

            // Act
            var result = _organDonationResultErrorMapper.Map(httpStatus);
            
            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
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
            var result = _organDonationResultErrorMapper.Map(httpStatus);
            
            // Assert
            result.Should().BeOfType<OrganDonationWithdrawResult.SystemError>()
                .Subject.Should().NotBeNull();
        }
        
        [TestMethod]
        public void MapOrganDonationAPIError_WhenRequestTimeoutErrorHttpStatus_MapsToSearchTimeoutError()
        { 
            // Act
            var result = _organDonationResultErrorMapper.Map(HttpStatusCode.RequestTimeout);
            
            // Assert
            result.Should().BeOfType<OrganDonationWithdrawResult.Timeout>()
                .Subject.Should().NotBeNull();
        }
        
        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        public void MapOrganDonationAPIError_WhenNonRecoverableErrorHttpStatus_MapsToBadGatewayErrorWithoutRetry(HttpStatusCode httpStatus)
        {
            // Arrange           
            var response = new PfsErrorResponse
            {
                ErrorCode = 0,
                ErrorMessage = "A non-recoverable exception has occurred processing the request"
            };
            
            var expectedErrorResult = new OrganDonationWithdrawResult.UpstreamError(response);

            // Act
            var result = _organDonationResultErrorMapper.Map(httpStatus);
            
            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
        }
    }
}