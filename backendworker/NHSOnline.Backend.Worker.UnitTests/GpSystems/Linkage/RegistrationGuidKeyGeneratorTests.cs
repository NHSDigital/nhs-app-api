using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Linkage
{
    [TestClass]
    public class RegistrationGuidKeyGeneratorTests
    {
        private RegistrationGuidKeyGenerator _systemUnderTest;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<RegistrationGuidKeyGenerator>();
        }
        
        [TestMethod]
        public void GenerateRegistrationKey_Returns_OK()
        {
            const string accountId = "1234";
            const string odsCode = "boom";
            const string linkageKey = "Bap";

            var result = _systemUnderTest.GenerateRegistrationKey(
                accountId, odsCode, linkageKey);

            result.Should().Be(odsCode + accountId + linkageKey);
        }
        
        [TestMethod]
        public void GenerateRegistrationKey_null_AccountId()
        {
            const string accountId = null;
            const string odsCode = "boom";
            const string linkageKey = "Bap";

            var result = _systemUnderTest.GenerateRegistrationKey(
                accountId, odsCode, linkageKey);

            result.Should().Be(odsCode + accountId + linkageKey);
        }
        
        [TestMethod]
        public void GenerateRegistrationKey_null_odsCode()
        {
            const string accountId = "1234";
            const string odsCode = null;
            const string linkageKey = "Bap";

            var result = _systemUnderTest.GenerateRegistrationKey(
                accountId, odsCode, linkageKey);

            result.Should().Be(odsCode + accountId + linkageKey);
        }
        
        [TestMethod]
        public void GenerateRegistrationKey_null_linkageKEy()
        {
            const string accountId = "1234";
            const string odsCode = "boom";
            const string linkageKey = null;

            var result = _systemUnderTest.GenerateRegistrationKey(
                accountId, odsCode, linkageKey);

            result.Should().Be(odsCode + accountId + linkageKey);
        }
        
        [TestMethod]
        public void GenerateRegistrationKey_all_null()
        {
            const string accountId = null;
            const string odsCode = "";
            const string linkageKey = null;

            Action act = () => _systemUnderTest.GenerateRegistrationKey(accountId, odsCode, linkageKey);

            act.Should().Throw<ArgumentException>()
                .WithMessage("need to provide values to create key");
        }
    }
}