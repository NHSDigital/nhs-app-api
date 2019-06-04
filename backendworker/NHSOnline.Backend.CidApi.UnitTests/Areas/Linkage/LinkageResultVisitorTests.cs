using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.CidApi.Areas.Linkage;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Linkage
{
    [TestClass]
    public class LinkageResultVisitorTests
    {
        private IFixture _fixture;
        private LinkageResultVisitor _visitorUnderTest;
        private Mock<HttpContext> _mockHttpContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Freeze<Mock<ILoggerFactory>>();

            _mockHttpContext = new Mock<HttpContext>();

            var mockAuthenticationService = new Mock<IAuthenticationService>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationService.Object);
            _mockHttpContext.SetupGet(x => x.RequestServices).Returns(mockServiceProvider.Object);

            _visitorUnderTest = _fixture.Create<LinkageResultVisitor>();
        }

        [TestMethod]
        [DataRow(Code.PatientNotRegisteredAtThisPractice, StatusCodes.Status404NotFound)]
        [DataRow(Code.NoSelfAssociatedUserExistWithThisPatient, StatusCodes.Status404NotFound)]
        [DataRow(Code.PracticeNotLive, StatusCodes.Status400BadRequest)]
        [DataRow(Code.PatientArchived, StatusCodes.Status403Forbidden)]
        [DataRow(Code.UnderMinimumAgeOrNonCompetent, StatusCodes.Status403Forbidden)]
        [DataRow(Code.UserSelfAssociatedAccountIsArchived, StatusCodes.Status403Forbidden)]
        public async Task SuccessfulMappings(Code code, int expectedStatusCode)
        {
            // Arrange
            var errorCase = new LinkageResult.ErrorCase(code);

            // Act
            var visited = await errorCase.Accept(_visitorUnderTest);

            // Assert
            visited.Should().NotBeNull();
            var result = visited.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(expectedStatusCode);
        }

        [TestMethod]
        [DataRow(Code.PatientNotRegisteredAtThisPractice, StatusCodes.Status404NotFound)]
        [DataRow(Code.UnderMinimumAgeOrNonCompetent, StatusCodes.Status403Forbidden)]
        public async Task SuccessfulMappingsPost(Code code, int expectedStatusCode)
        {
            // Arrange
            var errorCase = new LinkageResult.ErrorCase(code);

            // Act
            var visited = await errorCase.Accept(_visitorUnderTest);

            // Assert
            visited.Should().NotBeNull();
            var result = visited.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
