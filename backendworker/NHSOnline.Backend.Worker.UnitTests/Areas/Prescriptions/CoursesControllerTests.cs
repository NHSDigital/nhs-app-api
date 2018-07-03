using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Prescriptions;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class CoursesControllerTests
    {
        private CoursesController _systemUnderTest;
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

            _systemUnderTest = _fixture.Create<CoursesController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var courseService = new Mock<ICourseService>();

            var coursesGetResponse = new CourseListResponse();

            var getCoursesResult = new GetCoursesResult.SuccessfullyRetrieved(coursesGetResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetCourseService())
                .Returns(courseService.Object);

            courseService.Setup(x => x.GetCourses(_userSession)).Returns(Task.FromResult((GetCoursesResult)getCoursesResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetCourseService());
            courseService.Verify(x => x.GetCourses(_userSession));
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(coursesGetResponse);
        }
    }
}