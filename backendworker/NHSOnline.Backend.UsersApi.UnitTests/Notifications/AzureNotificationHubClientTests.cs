using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;
using Notification = NHSOnline.Backend.UsersApi.Notifications.Models.Notification;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubClientTests
    {
        private Mock<IAzureNotificationHubClientWrapper> _mockAzureNotificationHubClientWrapper;
        private AzureNotificationHubClient _systemUnderTest;

        private const string DevicePns = "devicePns";
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAzureNotificationHubClientWrapper = new Mock<IAzureNotificationHubClientWrapper>(MockBehavior.Strict);
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

        [TestMethod]
        public async Task FindInstallationIdentifiersByNhsLoginId_FoundRegistrationsWithInstallationTags_ReturnsInstallationIds()
        {
            // Arrange
            var registrationDescriptions = new[]
            {
                GenerateRegistration("registration1",
                    new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" }),
                GenerateRegistration("registration3",
                    new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab13}" }),
            };

            var expectedResult = new[]
            {
                "fe7312a9-43dc-46f6-9727-03b3ddecab12",
                "fe7312a9-43dc-46f6-9727-03b3ddecab13"
            };

            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByTagAsync($"nhsLoginId:{NhsLoginId}", 100))
                .ReturnsAsync(registrationDescriptions);

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiersByNhsLoginId(NhsLoginId);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task FindInstallationIdentifiersByNhsLoginId_FoundDuplicateInstallationTags_ReturnsUniqueInstallationIds()
        {
            // Arrange
            var registrationDescriptions = new[]
            {
                GenerateRegistration("registration1",
                    new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" }),
                GenerateRegistration("registration1",
                    new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" }),
            };

            var expectedResult = new[] { "fe7312a9-43dc-46f6-9727-03b3ddecab12" };

            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByTagAsync($"nhsLoginId:{NhsLoginId}", 100))
                .ReturnsAsync(registrationDescriptions);

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiersByNhsLoginId(NhsLoginId);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task FindInstallationIdentifiersByNhsLoginId_FoundSomeNonInstallationTags_ReturnsOnlyInstallationIds()
        {
            // Arrange
            var registrationDescriptions = new[]
            {
                GenerateRegistration("registration1",
                    new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" }),
                GenerateRegistration("registration1",
                    new[] { "nhsLoginId:fe7312a9-43dc-46f6-9727-03b3ddecab13" }),
            };

            var expectedResult = new[] { "fe7312a9-43dc-46f6-9727-03b3ddecab12" };

            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByTagAsync($"nhsLoginId:{NhsLoginId}", 100))
                .ReturnsAsync(registrationDescriptions);

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiersByNhsLoginId(NhsLoginId);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task FindInstallationIdentifiersByNhsLoginId_FoundRegistrationWithMultipleTags_ReturnsOnlyInstallationIds()
        {
            // Arrange
            var registrationDescriptions = new[]
            {
                GenerateRegistration("registration1",
                    new[]
                    {
                        "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}",
                        "nhsLoginId:fe7312a9-43dc-46f6-9727-03b3ddecab13",
                        "odsCode:AA1234"
                    }),
            };

            var expectedResult = new[] { "fe7312a9-43dc-46f6-9727-03b3ddecab12" };

            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByTagAsync($"nhsLoginId:{NhsLoginId}", 100))
                .ReturnsAsync(registrationDescriptions);

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiersByNhsLoginId(NhsLoginId);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task FindInstallationIdentifiersByNhsLoginId_FoundNoRegistrations_ReturnsEmptyCollection()
        {
            // Arrange
            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.GetRegistrationsByTagAsync($"nhsLoginId:{NhsLoginId}", 100))
                .ReturnsAsync(Array.Empty<RegistrationDescription>());

            // Act
            var result = await _systemUnderTest.FindInstallationIdentifiersByNhsLoginId(NhsLoginId);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();

            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task SendNotification_ForNhsLoginId_SendsNotification()
        {
            // Arrange
            var notification = new Notification
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = new Uri("http://www.example.com")
            };

            var expectedProperties = new Dictionary<string, string>
            {
                { "title", "title" },
                { "subtitle", "subtitle" },
                { "body", "body" },
                { "url", "http://www.example.com/" }
            };

            _mockAzureNotificationHubClientWrapper
                .Setup(x => x.SendTemplateNotificationAsync
                (
                    It.Is<IDictionary<string, string>>(y => !y.Except(expectedProperties).Any()),
                    $"nhsLoginId:{NhsLoginId}")
                )
                .ReturnsAsync(new NotificationOutcome());

            // Act
            await _systemUnderTest.SendNotification(NhsLoginId, notification);

            // Assert
            _mockAzureNotificationHubClientWrapper.VerifyAll();
        }

        private RegistrationDescription GenerateRegistration(string registrationId, string[] tags) => new FcmRegistrationDescription(DevicePns, tags)
        {
            RegistrationId = registrationId
        };
    }
}