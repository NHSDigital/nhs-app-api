using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Prescriptions;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class CoursesControllerTests
    {
        private CoursesController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IBridgeFactory> _mockBridgeFactory;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _mockBridgeFactory = _fixture.Freeze<Mock<IBridgeFactory>>();
            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _systemUnderTest = _fixture.Create<CoursesController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockBridge = new Mock<IBridge>();
            var courseService = new Mock<ICourseService>();

            var coursesGetResponse = new CourseListResponse();

            var getCoursesResult = new GetCoursesResult.SuccessfullyRetrieved(coursesGetResponse);

            // Arrange
            _mockBridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(mockBridge.Object);

            mockBridge.Setup(x => x.GetCourseService())
                .Returns(courseService.Object);

            courseService.Setup(x => x.Get(_userSession)).Returns(Task.FromResult((GetCoursesResult)getCoursesResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockBridgeFactory.Verify(x => x.CreateBridge(_userSession.Supplier));
            mockBridge.Verify(x => x.GetCourseService());
            courseService.Verify(x => x.Get(_userSession));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as GetCoursesResult.SuccessfullyRetrieved;
            Assert.IsNotNull(value);
        }
    }
}