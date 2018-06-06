using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.MyRecord;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Demographics;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.MyRecord
{
    [TestClass]
    public class MyRecordControllerTests
    {
        private MyRecordController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _systemUnderTest = _fixture.Create<MyRecordController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_Returns_SuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var demographicsService = new Mock<IDemographicsService>();

            var demographicsResponse = new DemographicsResponse();

            var getDemographicsResult = new GetMyRecordResult.SuccessfullyRetrieved(demographicsResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetDemographicsService())
                .Returns(demographicsService.Object);

            demographicsService.Setup(x => x.Get(_userSession)).Returns(Task.FromResult((GetMyRecordResult) getDemographicsResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetDemographicsService());
            demographicsService.Verify(x => x.Get(_userSession));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as GetMyRecordResult.SuccessfullyRetrieved;
            Assert.IsNotNull(value);
        }
        
        [TestMethod]
        public async Task Get_Returns_Status403Forbidden_When_Patient_Does_Not_Have_Access_To_Data()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var demographicsService = new Mock<IDemographicsService>();

            var response = new GetMyRecordResult.UserHasNoAccess();

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetDemographicsService())
                .Returns(demographicsService.Object);

            demographicsService.Setup(x => x.Get(_userSession)).Returns(Task.FromResult((GetMyRecordResult) response));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            demographicsService.Verify();
        }
    }
}