using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Logging;
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
        private TppLinkagePostErrorMapper _mapperUnderTest;
        private IFixture _fixture;
        private Mock<ILogger<TppLinkageService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _mapperUnderTest = new TppLinkagePostErrorMapper();
            _logger = _fixture.Create<Mock<ILogger<TppLinkageService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapperUnderTest.Map(null, _logger.Object);

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
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>();
            var conflictResult = (LinkageResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be( Im1ConnectionErrorCodes.Code.ProblemLinkingAccount);
        }
        
        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK, "10");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.UnknownError>();
            var conflictResult = (LinkageResult.UnknownError) result;
            conflictResult.ErrorCode.Should().Be( Im1ConnectionErrorCodes.Code.UnknownError);
        }


        public TppClient.TppApiObjectResponse<AddNhsUserResponse> CreateResponse(
            HttpStatusCode statusCode,
            string errorCode)
        {
            var response = _fixture.Create<TppClient.TppApiObjectResponse<AddNhsUserResponse>>();

            response.StatusCode = statusCode;
            response.ErrorResponse.ErrorCode = errorCode;
            return response;
        }
    }
}