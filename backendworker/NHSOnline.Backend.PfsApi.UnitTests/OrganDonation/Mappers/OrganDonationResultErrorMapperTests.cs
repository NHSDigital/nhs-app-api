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
    public class OrganDonationResultErrorMapperTests
    {
        private IMapper<HttpStatusCode, OrganDonationResult> _organDonationResultErrorMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationResultErrorMapper = fixture.Create<OrganDonationResultErrorMapper>();
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
            
            var expectedErrorResult = new OrganDonationResult.SearchUpstreamError(response);

            // Act
            var result = _organDonationResultErrorMapper.Map(httpStatus);
            
            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.MethodNotAllowed)]
        [DataRow(HttpStatusCode.UnsupportedMediaType)]
        [DataRow(HttpStatusCode.UnprocessableEntity)]
        public void MapOrganDonationAPIError_WhenPassingRequestErrorHttpStatus_MapsToSearchError(HttpStatusCode httpStatus)
        { 
            // Act
            var result = _organDonationResultErrorMapper.Map(httpStatus);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrganDonationResult.SearchError>();
        }
        
        [TestMethod]
        public void MapOrganDonationAPIError_WhenRequestTimeoutErrorHttpStatus_MapsToSearchTimeoutError()
        { 
            // Act
            var result = _organDonationResultErrorMapper.Map(HttpStatusCode.RequestTimeout);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrganDonationResult.SearchTimeout>();
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
            
            var expectedErrorResult = new OrganDonationResult.SearchUpstreamError(response);

            // Act
            var result = _organDonationResultErrorMapper.Map(httpStatus);
            
            // Assert
            result.Should().BeEquivalentTo(expectedErrorResult);
        }
    }
}