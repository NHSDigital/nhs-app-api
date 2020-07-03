using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubClientTests
    {
        private Mock<IAzureNotificationHubClientWrapper> _mockAzureNotificationHubClientWrapper;
        private AzureNotificationHubClient _systemUnderTest;

        private const string DevicePns = "devicePns";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAzureNotificationHubClientWrapper = new Mock<IAzureNotificationHubClientWrapper>();
            _systemUnderTest = new AzureNotificationHubClient(_mockAzureNotificationHubClientWrapper.Object);
        }

        [TestMethod]
        public async Task FindInstallationIdentifiers_Success()
        {
            // Arrange
            var installationIds = new List<string>
                { "fe7312a9-43dc-46f6-9727-03b3ddecab11", "fe7312a9-43dc-46f6-9727-03b3ddecab12" };

            var expectedResult = new List<NotificationRegistrationItem>
            {
                new NotificationRegistrationItem
                {
                    Id = installationIds[0],
                    Type = NotificationRegistrationItem.RegistrationType.Installation
                },
                new NotificationRegistrationItem
                {
                    Id = installationIds[1],
                    Type = NotificationRegistrationItem.RegistrationType.Installation
                },
                new NotificationRegistrationItem
                {
                    Id = "RegistrationId2",
                    Type = NotificationRegistrationItem.RegistrationType.Registration
                },
                new NotificationRegistrationItem
                {
                    Id = "RegistrationId4",
                    Type = NotificationRegistrationItem.RegistrationType.Registration
                },
            };

            var registrations = new List<RegistrationDescription>
            {
                GenerateRegistration("RegistrationId1", new[]
                    { $"$InstallationId:{{{installationIds[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration("RegistrationId2", new[]
                    { "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c33288142A" }),
                GenerateRegistration("RegistrationId3", new[]
                    { $"$InstallationId:{{{installationIds[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration("RegistrationId4", new[]
                    { "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c33288142B" })
            };

            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByChannelAsync(DevicePns, It.IsAny<int>()))
                .ReturnsAsync(registrations);

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiers(DevicePns);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task FindInstallationIdentifiers_DuplicatedInstallationId_ReturnsDistinctInstallationIds()
        {
            // Arrange
            var installationIds = new List<string>
                { "fe7312a9-43dc-46f6-9727-03b3ddecab11", "fe7312a9-43dc-46f6-9727-03b3ddecab12" };

            var expectedResult = new List<NotificationRegistrationItem>
            {
                new NotificationRegistrationItem
                {
                    Id = installationIds[0],
                    Type = NotificationRegistrationItem.RegistrationType.Installation
                },
                new NotificationRegistrationItem
                {
                    Id = installationIds[1],
                    Type = NotificationRegistrationItem.RegistrationType.Installation
                },
            };

            var registrations = new List<RegistrationDescription>
            {
                GenerateRegistration("RegistrationId1", new[]
                    { $"$InstallationId:{{{installationIds[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration("RegistrationId2",new[]
                    { $"$InstallationId:{{{installationIds[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration("RegistrationId3",new[]
                    { $"$InstallationId:{{{installationIds[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration("RegistrationId4",new[]
                    { $"$InstallationId:{{{installationIds[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration("RegistrationId5",new[]
                    { $"$InstallationId:{{{installationIds[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration("RegistrationId6",new[]
                    { $"$InstallationId:{{{installationIds[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" })
            };

            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByChannelAsync(DevicePns, It.IsAny<int>()))
                .ReturnsAsync(registrations);

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiers(DevicePns);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task FindInstallationIdentifiers_GetsNoRegistrations_ReturnsEmpty()
        {
            // Arrange
            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByChannelAsync(DevicePns, It.IsAny<int>()))
                .ReturnsAsync( Enumerable.Empty<RegistrationDescription>() );

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiers(DevicePns);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEmpty();
        }

        private RegistrationDescription GenerateRegistration(string registrationId, string[] tags) => new FcmRegistrationDescription(DevicePns, tags)
        {
            RegistrationId = registrationId
        };
    }
}