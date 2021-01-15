using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.PfsApi.UnitTests.Session
{
    [TestClass]
    public class SessionExpiryCookieCreatorTests
    {
        private SessionExpiryCookieCreatorTestContext Context { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Context = new SessionExpiryCookieCreatorTestContext();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void GetSessionExpiryCookieToken_WhenTokenIsNullOrEmpty_ReturnsNull(string token)
        {
            // Arrange
            Context.ArrangeDateTimeUtcNow();
            Context.ArrangeSigning();
            Context.ArrangeJwtTokenGenerator(token);

            // Act
            var actualToken = Context.CreateSystemUnderTest().GetSessionExpiryCookieToken();

            // Assert
            actualToken.Should().BeNull();
        }

        [TestMethod]
        public void GetSessionExpiryCookieToken_WhenSigningThrowsException_ReturnsNull()
        {
            // Arrange
            Context.ArrangeDateTimeUtcNow();
            Context.ArrangeSigningException();

            // Act
            var actualToken = Context.CreateSystemUnderTest().GetSessionExpiryCookieToken();

            // Assert
            actualToken.Should().BeNull();
        }

        [TestMethod]
        public void GetSessionExpiryCookieToken_WhenTokenIsGenerated_ReturnsToken()
        {
            // Arrange
            Context.ArrangeDateTimeUtcNow();
            Context.ArrangeSigning();
            Context.ArrangeJwtTokenGenerator();

            // Act
            var actualToken = Context.CreateSystemUnderTest().GetSessionExpiryCookieToken();

            // Assert
            actualToken.Should().BeEquivalentTo(SessionExpiryCookieCreatorTestContext.JwtToken);
        }
    }
}