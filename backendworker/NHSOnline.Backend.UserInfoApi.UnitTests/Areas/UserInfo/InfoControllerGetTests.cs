using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public class InfoControllerGetTests
    {
        private IFixture _fixture;
        private InfoController _systemUnderTest;
        private Mock<IInfoService> _mockInfoService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            var mockHttpContext =  HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);

            _mockInfoService = _fixture.Freeze<Mock<IInfoService>>();

            _systemUnderTest = _fixture.Create<InfoController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }
       
        [TestMethod]
        public async Task Get_WithOdsCode_SuccessFoundMultiple()
        {
            // Arrange
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            var odsCode = _fixture.Create<string>();
            var nhsLoginIds = _fixture.Create<IEnumerable<string>>();
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.FoundMultiple(nhsLoginIds));

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(nhsLoginIds);
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_SuccessFoundMultiple()
        {
            // Arrange
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            var nhsNumber = _fixture.Create<string>();
            var nhsLoginIds = _fixture.Create<IEnumerable<string>>();
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.FoundMultiple(nhsLoginIds));

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(nhsLoginIds);
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_NotFound_ReturnsNotFound()
        {
            // Arrange
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            var nhsNumber = _fixture.Create<string>();
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }


        [TestMethod]
        public async Task Get_WithOdsCode_NotFound_ReturnsNotFound()
        {
            // Arrange
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            var odsCode = _fixture.Create<string>();
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_BadGateway_ReturnsBadGateway()
        {
            // Arrange
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            var nhsNumber = _fixture.Create<string>();
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }


        [TestMethod]
        public async Task Get_WithOdsCode_BadGateway_ReturnsBadGateway()
        {
            // Arrange
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            var odsCode = _fixture.Create<string>();
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_WithNhsNumberAndOdsCode_ReturnsBadRequest()
        {
            // Act
            var result = await _systemUnderTest.Get(_fixture.Create<string>(), _fixture.Create<string>());

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public async Task Get_WithoutNhsNumberAndOdsCode_ReturnsBadRequest(string value)
        {
            // Act
            var result = await _systemUnderTest.Get(value, value);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}