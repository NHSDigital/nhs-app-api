using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Users.UnitTests
{
    [TestClass]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class AzureNotificationHubConfigurationValidatorTests
    {
        private AzureNotificationHubConfigurationValidator _systemUnderTest;
        private List<AzureNotificationHubConfiguration> _configurations;

        private const string FullRange = "0123456789ABCDEF";
        private const string Empty = "";

        [TestInitialize]
        public void Setup()
        {
            _configurations = new List<AzureNotificationHubConfiguration>();

            _systemUnderTest = new AzureNotificationHubConfigurationValidator(
                new Mock<ILogger>().Object
            );
        }

        [TestMethod]
        public void Validate_SingleHub_EverythingValid_DoesNotThrowException()
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = FullRange, Write = FullRange }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().NotThrow();
        }

        [DataTestMethod]
        [DynamicData(nameof(SingleRangeData))]
        public void Validate_SingleHub_IncompleteReadRange_ThrowsException(string readRange, char missingCharacter)
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = readRange, Write = FullRange }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"No entry for read character [{missingCharacter}] found");
        }

        [DataTestMethod]
        [DynamicData(nameof(SingleRangeData))]
        public void Validate_SingleHub_IncompleteWriteRange_ThrowsException(string writeRange, char missingCharacter)
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = FullRange, Write = writeRange }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"No entry for write character [{missingCharacter}] found");
        }

        [DataTestMethod]
        [DynamicData(nameof(CharacterData))]
        public void Validate_SingleHub_RepeatReadCharacters_ThrowsException(char additionalCharacter)
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = FullRange + additionalCharacter, Write = FullRange }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"Multiple entries for read character [{additionalCharacter}] found in generation [1]");
        }

        [DataTestMethod]
        [DynamicData(nameof(CharacterData))]
        public void Validate_SingleHub_RepeatWriteCharacters_ThrowsException(char additionalCharacter)
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = FullRange, Write = FullRange + additionalCharacter }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"Multiple entries for write character [{additionalCharacter}] found");
        }

        [DataTestMethod]
        [DynamicData(nameof(MultipleRangesData))]
        public void Validate_MultipleHubs_IncompleteReadRange_ThrowsException(
            string firstRange,
            string secondRange,
            char missingCharacter
        )
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = firstRange, Write = FullRange },
                new HubSettings{ Generation = 1, Read = secondRange, Write = Empty }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"No entry for read character [{missingCharacter}] found");
        }

        [DataTestMethod]
        [DynamicData(nameof(MultipleRangesData))]
        public void Validate_MultipleHubs_IncompleteWriteRange_ThrowsException(
            string firstRange,
            string secondRange,
            char missingCharacter
        )
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = firstRange, Write = firstRange },
                new HubSettings{ Generation = 2, Read = secondRange + missingCharacter, Write = secondRange }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"No entry for write character [{missingCharacter}] found");
        }

        [DataTestMethod]
        [DynamicData(nameof(MultipleRangesData))]
        public void Validate_MultipleHubs_RepeatReadCharactersInSameGeneration_ThrowsException(
            string firstRange,
            string secondRange,
            char character
        )
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = firstRange + character, Write = firstRange + character },
                new HubSettings{ Generation = 1, Read = secondRange + character, Write = secondRange }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"Multiple entries for read character [{character}] found in generation [1]");
        }

        [TestMethod]
        public void Validate_MultipleHubs_RepeatReadCharactersInMultipleGenerations_DoesNotThrowException()
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = FullRange, Write = "0123456789" },
                new HubSettings{ Generation = 2, Read = FullRange, Write = "ABCDEF" }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().NotThrow();
        }

        [DataTestMethod]
        [DynamicData(nameof(MultipleRangesData))]
        public void Validate_MultipleHubs_RepeatWriteCharacters_ThrowsException(
            string firstRange,
            string secondRange,
            char character
        )
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = firstRange + character, Write = firstRange + character },
                new HubSettings{ Generation = 2, Read = secondRange, Write = secondRange + character }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"Multiple entries for write character [{character}] found");
        }

        [DataTestMethod]
        [DynamicData(nameof(MultipleRangesData))]
        public void Validate_MultipleHubs_WriteCharacterNotReadable_ThrowsException(
            string firstRange,
            string secondRange,
            char character
        )
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = FullRange, Write = firstRange },
                new HubSettings{ Generation = 2, Read = secondRange, Write = secondRange + character }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain($"Entry for write character [{character}] found that does not permit reading");
        }

        [TestMethod]
        public void Validate_MultipleHubs_OneWithNoValidWriteCharacters_DoesNotThrowsException()
        {
            SetupConfiguration(new[]
            {
                new HubSettings{ Generation = 1, Read = FullRange, Write = string.Empty },
                new HubSettings{ Generation = 2, Read = FullRange, Write = FullRange }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().NotThrow();
        }

        [TestMethod]
        public void Validate_MultipleHubs_EverythingValid_DoesNotThrowException()
        {
            SetupConfiguration(new[]
            {
                new HubSettings { Generation = 1, Read = FullRange, Write = "0123" },
                new HubSettings { Generation = 2, Read = "456789ABCDEF", Write = "4567" },
                new HubSettings { Generation = 3, Read = "89AB", Write = "89AB" },
                new HubSettings { Generation = 3, Read = "CDEF", Write = "CDEF" }
            });

            Action action = () => _systemUnderTest.Validate(_configurations);

            action.Should().NotThrow<ConfigurationNotValidException>();
        }

        private void SetupConfiguration(IEnumerable<HubSettings> settings)
        {
            _configurations.AddRange(settings.Select(x => new AzureNotificationHubConfiguration(
                "connectionString",
                "notificationHubPath",
                "sharedAccessKey",
                x.Read,
                x.Write,
                x.Generation
            )));
        }

        private class HubSettings
        {
            public int Generation { get; set; }
            public string Read { get; set; } = string.Empty;
            public string Write { get; set; } = string.Empty;
        }

        public static IEnumerable<object[]> CharacterData => FullRange.Select(x => new object[] { x });

        public static IEnumerable<object[]> SingleRangeData => FullRange.Select(x => new object[]
        {
            FullRange.Replace(x.ToString(), string.Empty, StringComparison.InvariantCulture),
            x
        });

        public static IEnumerable<object[]> MultipleRangesData => FullRange.Select((x, i) => new object[]
        {
            FullRange.Substring(0, i),
            FullRange.Substring(i + 1),
            x
        });
    }
}
