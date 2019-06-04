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
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkagePostLinkageErrorMapperTests
    {
        private EmisTestHelpers _helper;
        private EmisLinkagePostErrorMapper _mapperUnderTest;
        private Mock<ILogger<EmisLinkageService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _helper = new EmisTestHelpers(fixture);
            _mapperUnderTest = fixture.Create<EmisLinkagePostErrorMapper>();
            _logger = fixture.Freeze<Mock<ILogger<EmisLinkageService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapperUnderTest.Map(null, _logger.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithBadRequestValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<AddNhsUserResponse>(HttpStatusCode.BadRequest,
                1553);

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>();
            var conflictResult = (LinkageResult.ErrorCase)result;
            conflictResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnderMinimumAgeOrNonCompetent);
        }
        
        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<AddNhsUserResponse>(HttpStatusCode.BadRequest, 999);

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.BadRequest>();
            var conflictResult = (LinkageResult.BadRequest) result;
            conflictResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
        }
    }
}