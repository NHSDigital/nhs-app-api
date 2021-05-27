using System;
using System.Security.Cryptography;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Hasher;

namespace NHSOnline.Backend.Support.UnitTests.Hasher
{
    [TestClass]
    public class HashingServiceTests
    {
        private Mock<ISha512ProviderFactory> _sha512ProviderFactory;

        private HashingService _hashService;

        [TestInitialize]
        public void TestInitialize() => SetupMocks();

        private void SetupMocks(
            bool hashComputeShouldThrowException = false,
            bool hashComputeShouldKeepThrowingException = false,
            bool hashComputeShouldRecoverOnThirdCall = false,
            bool hashDisposeShouldThrowException = false)
        {
            _sha512ProviderFactory = new Mock<ISha512ProviderFactory>();

            var sha512Provider = new Mock<ISha512Provider>();
            var throwCount = 0;
            var shouldThrow = hashComputeShouldThrowException;

            sha512Provider.Setup(s => s.ComputeHash(It.IsAny<byte[]>()))
                .Returns(() =>
                {
                    if (!shouldThrow)
                    {
                        return Array.Empty<byte>();
                    }

                    shouldThrow = hashComputeShouldKeepThrowingException;
                    throwCount++;

                    if (throwCount == 2)
                    {
                        shouldThrow = !hashComputeShouldRecoverOnThirdCall;
                    }

                    throw new CryptographicException();
                });

            sha512Provider.Setup(s => s.Dispose())
                .Callback(() =>
                {
                    if (hashDisposeShouldThrowException)
                    {
                        throw new ObjectDisposedException("borked");
                    }
                });

            _sha512ProviderFactory.Setup(f => f.Build())
                .Returns(sha512Provider.Object);

            _hashService = new HashingService(_sha512ProviderFactory.Object, new NullLogger<HashingService>());
        }

        [TestMethod]
        public void CheckHashIsConsistent()
        {
            const string key = "A000024176050002NxdUgCyDUAVz6";

            var e1 = _hashService.Hash(key);
            Thread.Sleep(1000);
            var e2 = _hashService.Hash(key);

            e1.Should().BeEquivalentTo(e2);
        }

        [TestMethod]
        public void When_Hash_IsCreated_Initial_Death_Status_IsFalse() =>
            Assert.IsFalse(_hashService.IsDead);

        [TestMethod]
        public void When_Hash_IsCalled_And_Hash_ThrowsCryptoException_Then_New_Hash_IsBuilt()
        {
            SetupMocks(hashComputeShouldThrowException: true);

            _hashService.Hash("test-stuff");

            _sha512ProviderFactory.Verify(f => f.Build(), Times.Exactly(2));
        }

        [TestMethod]
        public void When_Hash_IsCalled_And_Hash_ThrowsCryptoException_And_HashDispose_ThrowsAnException_Then_New_Hash_IsBuilt()
        {
            SetupMocks(hashComputeShouldThrowException: true, hashDisposeShouldThrowException: true);

            _hashService.Hash("test-stuff");

            _sha512ProviderFactory.Verify(f => f.Build(), Times.Exactly(2));
        }

        [TestMethod]
        public void When_Hash_IsCalled_And_Hash_ThrowsCryptoException_And_Retry_Succeeds_Then_Death_Status_IsFalse()
        {
            SetupMocks(hashComputeShouldThrowException: true);

            _hashService.Hash("test-stuff");

            Assert.IsFalse(_hashService.IsDead);
        }

        [TestMethod]
        public void When_Hash_IsCalled_And_Hash_ContinuesToThrowCryptoException_After_New_Hash_IsBuilt_Then_Exception_IsThrown()
        {
            SetupMocks(hashComputeShouldThrowException: true, hashComputeShouldKeepThrowingException: true);

            Assert.ThrowsException<CryptographicException>(() =>
                _hashService.Hash("test-stuff")
            );
        }

        [TestMethod]
        public void When_Hash_IsCalled_And_Hash_ContinuesToThrowCryptoException_Then_Provider_IsBuiltTwice()
        {
            SetupMocks(hashComputeShouldThrowException: true, hashComputeShouldKeepThrowingException: true);

            Assert.ThrowsException<CryptographicException>(() =>
                _hashService.Hash("test-stuff")
            );

            _sha512ProviderFactory.Verify(f => f.Build(), Times.Exactly(2));
        }

        [TestMethod]
        public void When_Hash_IsCalled_And_Hash_ContinuesToThrowCryptoException_After_New_Hash_IsBuilt_Then_Service_IsMarkedAsDead()
        {
            SetupMocks(hashComputeShouldThrowException: true, hashComputeShouldKeepThrowingException: true);

            Assert.ThrowsException<CryptographicException>(() =>
                _hashService.Hash("test-stuff")
            );

            Assert.IsTrue(_hashService.IsDead);
        }

        [TestMethod]
        public void When_Hash_IsCalled_And_Hash_ContinuesToThrowCryptoException_AndNextCallSucceeds_Then_Service_IsMarkedAsDead()
        {
            SetupMocks(
                hashComputeShouldThrowException: true,
                hashComputeShouldKeepThrowingException: true,
                hashComputeShouldRecoverOnThirdCall: true);

            Assert.ThrowsException<CryptographicException>(() =>
                _hashService.Hash("test-stuff")
            );

            _hashService.Hash("something-funky");

            Assert.IsTrue(_hashService.IsDead);
        }
    }
}
