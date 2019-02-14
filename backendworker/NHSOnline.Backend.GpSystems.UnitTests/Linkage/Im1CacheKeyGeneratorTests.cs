using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support.Hasher;

namespace NHSOnline.Backend.GpSystems.UnitTests.Linkage
{
    [TestClass]
    public class Im1CacheKeyGeneratorTests
    {
        private Im1CacheKeyGenerator _systemUnderTest;
        private Mock<IHashingService> _hashingService;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _hashingService = _fixture.Freeze<Mock<IHashingService>>();
            _hashingService.Setup(hs => hs.Hash(It.IsAny<string>()))
                .Returns((string s) => s);

            _systemUnderTest = _fixture.Create<Im1CacheKeyGenerator>();
        }
        
        [TestMethod]
        public void GenerateCacheKey_AllParametersPassed_ReturnsCacheKey()
        {
            const string accountId = "1234";
            const string odsCode = "boom";
            const string linkageKey = "Bap";

            _hashingService.Reset();
            _hashingService.Setup(hs => hs.Hash(odsCode + accountId + linkageKey))
                .Returns((string s) => s)
                .Verifiable();

            var result = _systemUnderTest.GenerateCacheKey(
                accountId, odsCode, linkageKey);

            result.Should().Be(odsCode + accountId + linkageKey);
            _hashingService.Verify();
        }
        
        [DataTestMethod]
        [DataRow(null, "boom", "Bap")]
        [DataRow("1234", null, "Bap")]
        [DataRow("1234", "boom", null)]
        public void GenerateCacheKey_SomeParametersMissing_ReturnsCacheKey( string accountId, string odsCode, string linkageKey)
        {
            var result = _systemUnderTest.GenerateCacheKey(
                accountId, odsCode, linkageKey);

            result.Should().Be(odsCode + accountId + linkageKey);
        }
        
        [TestMethod]
        public void GenerateCacheKey_NoParametersPassed_ThrowsArgumentException()
        {
            const string accountId = null;
            const string odsCode = "";
            const string linkageKey = null;

            Action act = () => _systemUnderTest.GenerateCacheKey(accountId, odsCode, linkageKey);

            act.Should().Throw<ArgumentException>()
                .WithMessage("need to provide values to create key");
        }
    }
}