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
            var expectedResult = new List<string>
                { "fe7312a9-43dc-46f6-9727-03b3ddecab11", "fe7312a9-43dc-46f6-9727-03b3ddecab12" };

            var registrations = new List<RegistrationDescription>
            {
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" })
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
            var expectedResult = new List<string>
                { "fe7312a9-43dc-46f6-9727-03b3ddecab11", "fe7312a9-43dc-46f6-9727-03b3ddecab12" };

            var registrations = new List<RegistrationDescription>
            {
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[0]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" }),
                GenerateRegistration(new[]
                    { $"$InstallationId:{{{expectedResult[1]}}}", "nhsLoginId:3c90d2d7-5474-4acd-8722-c7c332881421" })
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

        private RegistrationDescription GenerateRegistration(string[] tags) => new FcmRegistrationDescription("RegistrationId", tags);
    }
}