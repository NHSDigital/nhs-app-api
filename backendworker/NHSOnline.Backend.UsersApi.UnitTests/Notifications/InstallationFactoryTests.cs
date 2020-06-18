using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class InstallationFactoryTests
    {
        private InstallationFactory _systemUnderTest;
        private Mock<IInstallationTemplateFactory> _mockInstallationTemplateFactory;
        private const string NhsLoginId = "NhsLoginId";
        private const string DevicePns = "DevicePns";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInstallationTemplateFactory = new Mock<IInstallationTemplateFactory>();
            _systemUnderTest = new InstallationFactory(_mockInstallationTemplateFactory.Object);
        }

        [TestMethod]
        [DataRow(DeviceType.Android, NotificationPlatform.Fcm)]
        [DataRow(DeviceType.Ios, NotificationPlatform.Apns)]
        public void Create_Success(DeviceType deviceType, NotificationPlatform platform)
        {
            // Arrange
            var installationTemplate = new InstallationTemplate
            {
                Body = "TemplateBody"
            };

            var expectedTemplates = new Dictionary<string, InstallationTemplate>
            {
                { $"{NhsLoginId}-PrimaryMessageTemplate", installationTemplate }
            };

            var expectedTags = new List<string>
            {
                $"{Constants.UsersConstants.NhsLoginIdTagPrefix}{Constants.UsersConstants.TagSeparator}{NhsLoginId}"
            };

            _mockInstallationTemplateFactory
                .Setup(x => x.Create(deviceType))
                .Returns(installationTemplate);

            // Act
            var result = _systemUnderTest.Create(DevicePns, deviceType, NhsLoginId);

            // Assert
            _mockInstallationTemplateFactory.VerifyAll();

            result.Should().NotBeNull();
            Guid.TryParse(result.InstallationId, out _).Should().BeTrue();
            result.PushChannel.Should().Be(DevicePns);
            result.Platform.Should().Be(platform);
            result.Templates.Should().BeEquivalentTo(expectedTemplates);
            result.Tags.Should().BeEquivalentTo(expectedTags);
        }
    }
}