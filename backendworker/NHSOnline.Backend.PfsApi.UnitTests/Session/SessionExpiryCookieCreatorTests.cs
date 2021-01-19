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
        public void CreateSessionExpiryToken_WhenTokenIsNullOrEmpty_ReturnsNull(string token)
        {
            // Arrange
            Context.ArrangeDateTimeUtcNow();
            Context.ArrangeSigning();
            Context.ArrangeJwtTokenGenerator(token);

            // Act
            var actualToken = Context.CreateSystemUnderTest().CreateSessionExpiryToken();

            // Assert
            actualToken.Should().BeNull();
        }

        [TestMethod]
        public void CreateSessionExpiryToken_WhenSigningThrowsException_ReturnsNull()
        {
            // Arrange
            Context.ArrangeDateTimeUtcNow();
            Context.ArrangeSigningException();

            // Act
            var actualToken = Context.CreateSystemUnderTest().CreateSessionExpiryToken();

            // Assert
            actualToken.Should().BeNull();
        }

        [TestMethod]
        public void CreateSessionExpiryToken_WhenTokenIsGenerated_ReturnsToken()
        {
            // Arrange
            Context.ArrangeDateTimeUtcNow();
            Context.ArrangeSigning();
            Context.ArrangeJwtTokenGenerator();

            // Act
            var actualToken = Context.CreateSystemUnderTest().CreateSessionExpiryToken();

            // Assert
            actualToken.Should().BeEquivalentTo(SessionExpiryCookieCreatorTestContext.JwtToken);
        }
    }
}