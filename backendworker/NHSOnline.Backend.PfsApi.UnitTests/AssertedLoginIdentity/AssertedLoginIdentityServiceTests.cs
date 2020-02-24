using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;

namespace NHSOnline.Backend.PfsApi.UnitTests.AssertedLoginIdentity
{
    [TestClass]
    public class AssertedLoginIdentityServiceTests
    {
        private IFixture _fixture;
        private Mock<ICitizenIdJwtHelper> _jwtHelper;

        private AssertedLoginIdentityService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _jwtHelper = _fixture.Freeze<Mock<ICitizenIdJwtHelper>>();

            _systemUnderTest = _fixture.Create<AssertedLoginIdentityService>();
        }

        [TestMethod]
        public void CreateJwt_JwtHelperReturnsToken_ReturnsSuccess()
        {
            // Arrange
            var idTokenJti = _fixture.Create<string>();
            var jwt = _fixture.Create<string>();
            _jwtHelper.Setup(x => x.CreateAssertedLoginIdentityJwt(idTokenJti)).Returns(jwt);

            // Act
            var result = _systemUnderTest.CreateJwtToken(idTokenJti);

            // Assert
            result.Should().BeOfType<CreateJwtResult.Success>().Subject
                .Response.Token.Should().Be(jwt);
        }

        [TestMethod]
        public void CreateJwt_JwtHelperThrows_LogsAndReturnsInternalServerError()
        {
            // Arrange
            var idTokenJti = _fixture.Create<string>();

            _jwtHelper.Setup(x => x.CreateAssertedLoginIdentityJwt(idTokenJti)).Throws<Exception>();

            // Act
            var result = _systemUnderTest.CreateJwtToken(idTokenJti);

            // Assert
            result.Should().BeOfType<CreateJwtResult.InternalServerError>();
        }
    }
}