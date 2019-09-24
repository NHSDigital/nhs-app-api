using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public class DeviceIdGeneratorTests
    {
        private IFixture _fixture;
        private DeviceIdGenerator _systemUnderTest;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            var mockLogger = _fixture.Freeze<Mock<ILogger<DeviceRepositoryService>>>();
            
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>()),
            });
            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);
            _systemUnderTest = _fixture.Create<DeviceIdGenerator>();
        }

        [TestMethod]
        public void Generate_WithRequestAndAccessToken_ReturnsDeviceId()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
                
            // Act
            var result = _systemUnderTest.Generate(_accessToken, request);
            
            // Assert
            result.Should().Be($"{_accessToken.Subject}-{request.DevicePns}");
        }

        [TestMethod]
        public void Generate_WithDevicePnsAndAccessToken_ReturnsDeviceId()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
                
            // Act
            var result = _systemUnderTest.Generate(_accessToken, devicePns);
            
            // Assert
            result.Should().Be($"{_accessToken.Subject}-{devicePns}");
        }
    }
}