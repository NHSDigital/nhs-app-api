using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkagePostLinkageErrorMapperTests
    {
        private IFixture _fixture;
        private Mock<ILogger<TppLinkageService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _logger = _fixture.Create<Mock<ILogger<TppLinkageService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            // Act
            Action act = () => TppLinkagePostErrorMapper.Map(null, _logger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithOkayWithErrorCodeValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(
                HttpStatusCode.OK,
                "6");

            // Act
            var result = TppLinkagePostErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount);
        }
        
        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK, "10");

            // Act
            var result = TppLinkagePostErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }
        
        private TppApiObjectResponse<AddNhsUserResponse> CreateResponse(
            HttpStatusCode statusCode,
            string errorCode)
        {
            var response = _fixture.Create<TppApiObjectResponse<AddNhsUserResponse>>();

            response.StatusCode = statusCode;
            response.ErrorResponse.ErrorCode = errorCode;
            return response;
        }
    }
}