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
        [DataRow(InternalCode.PatientNotRegisteredAtThisPractice, StatusCodes.Status404NotFound)]
        [DataRow(InternalCode.NoSelfAssociatedUserExistWithThisPatient, StatusCodes.Status404NotFound)]
        [DataRow(InternalCode.PracticeNotLive, StatusCodes.Status400BadRequest)]
        [DataRow(InternalCode.PatientArchived, StatusCodes.Status403Forbidden)]
        [DataRow(InternalCode.UnderMinimumAgeOrNonCompetent, StatusCodes.Status403Forbidden)]
        [DataRow(InternalCode.UserSelfAssociatedAccountIsArchived, StatusCodes.Status403Forbidden)]
        public async Task SuccessfulMappings(InternalCode code, int expectedStatusCode)
        {
            // Arrange
            var errorCase = new LinkageResult.ErrorCase(code);

            // Act
            var visited = await errorCase.Accept(_visitorUnderTest);

            // Assert
            visited.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(expectedStatusCode);
        }

        [TestMethod]
        [DataRow(InternalCode.PatientNotRegisteredAtThisPractice, StatusCodes.Status404NotFound)]
        [DataRow(InternalCode.UnderMinimumAgeOrNonCompetent, StatusCodes.Status403Forbidden)]
        public async Task SuccessfulMappingsPost(InternalCode code, int expectedStatusCode)
        {
            // Arrange
            var errorCase = new LinkageResult.ErrorCase(code);

            // Act
            var visited = await errorCase.Accept(_visitorUnderTest);

            // Assert
            visited.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
