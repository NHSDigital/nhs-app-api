using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.UsersApi.Notifications.Extensions;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications.Extensions
{
    [TestClass]
    public class RegistrationDescriptionExtensionsTests
    {
        private const string DevicePns = "DevicePns";

        [TestMethod]
        public void InstallationIds_RegistrationDescriptionsWithInstallationTags_ReturnsInstallationIds()
        {
            // Arrange
            var descriptions = new[]
            {
                GenerateRegistration("registration1", new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" }),
                GenerateRegistration("registration2", new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab13}" })
            };

            var expectedResult = new[]
            {
                "fe7312a9-43dc-46f6-9727-03b3ddecab12",
                "fe7312a9-43dc-46f6-9727-03b3ddecab13"
            };

            // Act
            var result = descriptions.InstallationIds();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void InstallationIds_DuplicateInstallationTags_ReturnsUniqueInstallationIds()
        {
            // Arrange
            var descriptions = new[]
            {
                GenerateRegistration("registration1", new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" }),
                GenerateRegistration("registration1", new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" })
            };

            var expectedResult = new[]
            {
                "fe7312a9-43dc-46f6-9727-03b3ddecab12"
            };

            // Act
            var result = descriptions.InstallationIds();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void InstallationIds_SomeNonInstallationTags_ReturnsOnlyInstallationIds()
        {
            // Arrange
            var descriptions = new[]
            {
                GenerateRegistration("registration1", new[] { "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}" }),
                GenerateRegistration("registration1", new[] { "nhsLoginId:fe7312a9-43dc-46f6-9727-03b3ddecab13" })
            };

            var expectedResult = new[]
            {
                "fe7312a9-43dc-46f6-9727-03b3ddecab12"
            };

            // Act
            var result = descriptions.InstallationIds();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void InstallationIds_RegistrationWithMultipleTags_ReturnsOnlyInstallationIds()
        {
            // Arrange
            var tags = new[]
            {
                "$InstallationId:{fe7312a9-43dc-46f6-9727-03b3ddecab12}",
                "nhsLoginId:fe7312a9-43dc-46f6-9727-03b3ddecab13",
                "odsCode:AA1234"
            };

            var descriptions = new[]
            {
                GenerateRegistration("registration1", tags)
            };

            var expectedResult = new[]
            {
                "fe7312a9-43dc-46f6-9727-03b3ddecab12"
            };

            // Act
            var result = descriptions.InstallationIds();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void InstallationIds_NoRegistrations_ReturnsEmptyCollection()
        {
            // Arrange
            var descriptions = Enumerable.Empty<RegistrationDescription>();

            // Act
            var result = descriptions.InstallationIds();

            // Assert
            result.Should().BeEmpty();
        }

        private static RegistrationDescription GenerateRegistration(string registrationId, IEnumerable<string> tags)
        {
            return new FcmRegistrationDescription(DevicePns, tags)
            {
                RegistrationId = registrationId
            };
        }
    }
}
