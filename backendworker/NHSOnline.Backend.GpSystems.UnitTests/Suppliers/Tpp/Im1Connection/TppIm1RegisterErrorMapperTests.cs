using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Im1Connection
{
    [TestClass]
    public class TppIm1RegisterErrorMapperTests
    {
        private TppIm1RegisterErrorMapper _mapperUnderTest;
        private IFixture _fixture;
        private Mock<ILogger<TppIm1ConnectionService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _mapperUnderTest = new TppIm1RegisterErrorMapper();
            _logger = _fixture.Create<Mock<ILogger<TppIm1ConnectionService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapperUnderTest.Map(null, _logger.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithOkayWithErrorValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK,
                "8");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>();
            var conflictResult = (Im1ConnectionRegisterResult.ErrorCase)result;
            conflictResult.ErrorCode.Should().Be((int)Im1ConnectionErrorCodes.Code.InvalidLinkageDetailsTpp);
        }

        [TestMethod]
        public void Map_WithOkayWithUnknownErrorValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK, "10");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.UnknownError>();
            var conflictResult = (Im1ConnectionRegisterResult.UnknownError)result;
            conflictResult.ErrorCode.Should().Be((int)Im1ConnectionErrorCodes.Code.UnknownError);
        }

        public TppClient.TppApiObjectResponse<LinkAccountReply> CreateResponse(
            HttpStatusCode statusCode,
            string errorCode)
        {
            var response = _fixture.Create<TppClient.TppApiObjectResponse<LinkAccountReply>>();

            response.StatusCode = statusCode;
            response.ErrorResponse.ErrorCode = errorCode;
            return response;
        }
    }
}