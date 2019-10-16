using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.Areas.LinkedAccounts;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.LinkedAccounts
{
    [TestClass]
    public class LinkedAccountControllerTests
    {
        private LinkedAccountsController _systemUnderTest;
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

            _systemUnderTest = _fixture.Create<LinkedAccountsController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var linkedAccountServices = new Mock<ILinkedAccountsService>();
            LinkedAccountsResult linkedAccountResult = new LinkedAccountsResult.Success(
                _fixture.Create<GetLinkedAccountsResponse>());
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(linkedAccountServices.Object);

            linkedAccountServices.Setup(x => x.GetLinkedAccounts(_userSession.GpUserSession))
                .Returns(Task.FromResult(linkedAccountResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            var subject = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            subject.StatusCode.Value.Should().Equals(HttpStatusCode.OK);
            subject.Value.Should().NotBeNull();
        }
        
    }
}