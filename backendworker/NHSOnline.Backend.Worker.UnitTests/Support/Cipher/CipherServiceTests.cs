using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Cipher;

namespace NHSOnline.Backend.Worker.UnitTests.Support.Cipher
{
    [TestClass]
    public class CipherServiceTests
    {
        private const string TempConfigurationFilePath = "tempEncryptKey.txt";
        private CipherService _cipherService;

        [TestInitialize]
        public void TestInitialise()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config[CipherConfiguration.CipherKeyFilePathConfigurationName]).Returns(TempConfigurationFilePath);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(configurationMock.Object);
            serviceCollection.AddSingleton<CipherConfiguration>();
            var services = serviceCollection.BuildServiceProvider();

            File.WriteAllText(TempConfigurationFilePath, "ENCRYPT_KEY__________________128");
            _cipherService = ActivatorUtilities.CreateInstance<CipherService>(services);
            File.Delete(TempConfigurationFilePath);
        }

        [TestMethod]
        public void Encrypt_ShouldEncryptStringData()
        {
            // Arrange
            const string dataToEncrypt = "some_data_to_encrypt";

            // Act
            var encryptedData = _cipherService.Encrypt(dataToEncrypt);

            // Assert
            encryptedData.Should().NotBeNullOrEmpty();
            encryptedData.Should().NotBeSameAs(dataToEncrypt);
        }

        [TestMethod]
        public void Decrypt_ShouldDecryptEncryptedData()
        {
            // Arrange
            const string dataToEncrypt = "some_data_to_encrypt";
            var encryptedData = _cipherService.Encrypt(dataToEncrypt);

            // Act
            var decryptedData = _cipherService.Decrypt(encryptedData);

            // Assert
            decryptedData.Should().NotBeNullOrEmpty();
            decryptedData.Should().BeEquivalentTo(dataToEncrypt);
        }

        [TestMethod]
        public void Decrypt_ShouldDecryptDataFromOtherSource()
        {
            // This is a fixed piece of data, encrypted with the same key but different time, can still be decrypted.
            var decryptedData = _cipherService.Decrypt("AdSVtUvukdl+n5kFg9l/Jw==;G9WLkUUNzphidPzMg1qgquXHd+GVsKVZMHPqlz2YR80=");

            // Assert
            decryptedData.Should().NotBeNullOrEmpty();
            decryptedData.Should().BeEquivalentTo("some_data_to_encrypt");
        }
    }
}
