using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Worker.UnitTests
{
    [TestClass]
    public class CipherServiceTests
    {
        private CipherService _cipherService;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            var services = serviceCollection.BuildServiceProvider();
            _cipherService = ActivatorUtilities.CreateInstance<CipherService>(services);
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
    }
}
