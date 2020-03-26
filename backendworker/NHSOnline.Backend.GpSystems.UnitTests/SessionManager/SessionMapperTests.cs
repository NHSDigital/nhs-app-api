using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.SessionManager.Model;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionMapperTests
    {
        private IFixture _fixture;
        private Mock<IAntiforgery> _mockAntiForgery;
        private Mock<HttpContext> _mockHttpContext;
        private string _csrfToken;
        private string _im1ConnectionToken;

        private SessionMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockAntiForgery = _fixture.Freeze<Mock<IAntiforgery>>();
            _mockHttpContext = _fixture.Freeze<Mock<HttpContext>>();

            _csrfToken = _fixture.Create<string>();
            _im1ConnectionToken = _fixture.Create<string>();

            _mockAntiForgery
                .Setup(x => x.GetTokens(_mockHttpContext.Object))
                .Returns(new AntiforgeryTokenSet(_csrfToken, "", "", ""))
                .Verifiable();

            _systemUnderTest = _fixture.Create<SessionMapper>();
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsMappedUserSession()
        {
            // Arrange
            var gpSessionManagerCitizenIdUserSession = _fixture.Create<GpSessionManagerCitizenIdUserSession>();

            var citizenIdUserSession = new CitizenIdUserSession
            {
                AccessToken = gpSessionManagerCitizenIdUserSession.AccessToken,
                FamilyName = gpSessionManagerCitizenIdUserSession.FamilyName,
                IdTokenJti = gpSessionManagerCitizenIdUserSession.IdTokenJti,
                DateOfBirth = gpSessionManagerCitizenIdUserSession.DateOfBirth,
            };

            var gpUserSession = _fixture.Create<GpUserSession>();

            var expectedResult = new UserSession
            {
                CsrfToken = _csrfToken,
                GpUserSession = gpUserSession,
                CitizenIdUserSession = citizenIdUserSession,
                Im1ConnectionToken = _im1ConnectionToken
            };

            // Act
            var result = _systemUnderTest.Map(
                _mockHttpContext.Object, gpUserSession, gpSessionManagerCitizenIdUserSession, _im1ConnectionToken);

            // Assert
            result.Should().BeEquivalentTo(
                expectedResult,
                options => options.Excluding(x => x.OrganDonationSessionId));
            result.OrganDonationSessionId.Should().NotBeEmpty();
            _mockAntiForgery.Verify();
        }
    }
}