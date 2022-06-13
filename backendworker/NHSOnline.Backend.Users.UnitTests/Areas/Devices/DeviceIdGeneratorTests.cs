using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.Users.UnitTests.Areas.Devices
{
    [TestClass]
    public class DeviceIdGeneratorTests
    {
        private DeviceIdGenerator _systemUnderTest;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            var mockLogger = new Mock<ILogger<DeviceRepositoryService>>();

            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim("nhs_number", "9876543210"),
            });
            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);
            _systemUnderTest = new DeviceIdGenerator();
        }

        [TestMethod]
        public void Generate_WithRequestAndAccessToken_ReturnsDeviceId()
        {
            // Arrange
            var request = new RegisterDeviceRequest
            {
                DevicePns = Guid.NewGuid().ToString(),
                DeviceType = DeviceType.Android
            };

            // Act
            var result = _systemUnderTest.Generate(_accessToken, request);

            // Assert
            result.Should().Be($"{_accessToken.Subject}-{request.DevicePns}");
        }

        [TestMethod]
        public void Generate_WithDevicePnsAndAccessToken_ReturnsDeviceId()
        {
            // Arrange
            var devicePns = Guid.NewGuid().ToString();

            // Act
            var result = _systemUnderTest.Generate(_accessToken, devicePns);

            // Assert
            result.Should().Be($"{_accessToken.Subject}-{devicePns}");
        }
    }
}