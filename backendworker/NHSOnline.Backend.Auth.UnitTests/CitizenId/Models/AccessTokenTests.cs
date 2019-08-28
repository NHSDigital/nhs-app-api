using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId.Models
{
    [TestClass]
    public class AccessTokenTests
    {
        private IFixture _fixture;
        private Mock<ILogger> _loggerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger>>();
        }

        [TestMethod]
        public void Parse_InvalidAccessToken_ThrowsException()
        {
            // Arrange
            var accessToken = _fixture.Create<string>();

            // Act
            Action act = () => AccessToken.Parse(_loggerMock.Object, accessToken);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*JWT is not well formed*");
        }

        [TestMethod]
        public void Parse_WithNoClaims_ThrowsException()
        {
            // Arrange
            var accessToken = JwtToken.Generate();

            // Act
            Action act = () => AccessToken.Parse(_loggerMock.Object, accessToken);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("subject", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("nhsNumber", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Parse_WithNoSubject_ThrowsException()
        {
            // Arrange
            var accessToken = JwtToken.Generate(new[]
            {
                new Claim("nhs_number", "9987574309"),
            });

            // Act
            Action act = () => AccessToken.Parse(_loggerMock.Object, accessToken);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("subject", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Parse_WithNoNhsNumber_ThrowsException()
        {
            // Arrange
            var accessToken = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
            });

            // Act
            Action act = () => AccessToken.Parse(_loggerMock.Object, accessToken);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("nhsNumber", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Parse_ValidAccessTokenString_ReturnsParsedModel()
        {
            // Arrange
            var subject = _fixture.Create<string>();
            var nhsNumber = "9987574309";
            var accessToken = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim("nhs_number", "9987574309"),
            });

            // Act
            var result = AccessToken.Parse(_loggerMock.Object, accessToken);

            // Assert
            result.Should().NotBeNull();
            result.Subject.Should().Be(subject);
            result.NhsNumber.Should().Be(nhsNumber);
        }
    }
}