using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.Areas.Session.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionControllerTests
    {
        private SessionController _systemUnderTest;
        private static IFixture _fixture;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = _fixture.Create<SessionController>();
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsUserNameInBody()
        {
            // Arrange
            var request = _fixture.Create<UserSessionRequest>();

            // Act
            var result = await _systemUnderTest.Post(request);

            // Assert
            result.Should().BeAssignableTo<CreatedResult>();
            
            var createdResult = result as CreatedResult;
            // ReSharper disable once PossibleNullReferenceException
            createdResult.Location.Should().BeEmpty();
            var responseObject = createdResult.Value;
            responseObject.Should().BeAssignableTo<UserSessionResponse>();

            var userSessionResponse = responseObject as UserSessionResponse;
            // ReSharper disable once PossibleNullReferenceException
            userSessionResponse.FamilyName.Should().Be("Doyle");
            userSessionResponse.GivenName.Should().Be("James");
        }
    }
}