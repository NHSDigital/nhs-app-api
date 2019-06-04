using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public class NominatedPharmacyConfigTests
    {
        private NominatedPharmacyConfig _nominatedPharmacyConfig;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<NominatedPharmacyConfig>> _mockLogger;
        private IFixture _fixture;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Freeze<Mock<ILogger<NominatedPharmacyConfig>>>();
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [DataTestMethod]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "false")]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "FALSE")]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "False")]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "any non true value")]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "")]
        [DataRow("NOMINATED_PHARMACY_ENABLED", null)]
        public void TestIsNominatedPharmacyEnabledFalse(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.IsNominatedPharmacyEnabled, false);
        }
        
        [DataTestMethod]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "true")]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "TRUE")]
        [DataRow("NOMINATED_PHARMACY_ENABLED", "True")]
        public void TestIsNominatedPharmacyEnabledTrue(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.IsNominatedPharmacyEnabled, true);
        }

        [DataTestMethod]
        [DataRow("SPINE_ACCREDITED_SYSTEM_ID_FROM", "test")]
        [DataRow("SPINE_ACCREDITED_SYSTEM_ID_FROM", "")]
        [DataRow("SPINE_ACCREDITED_SYSTEM_ID_FROM", null)]
        public void TestSpineAccreditedSystemIdFrom(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.SpineAccreditedSystemIdFrom, value);
        }
        
        [DataTestMethod]
        [DataRow("SPINE_ACCREDITED_SYSTEM_ID_TO", "test")]
        [DataRow("SPINE_ACCREDITED_SYSTEM_ID_TO", "")]
        [DataRow("SPINE_ACCREDITED_SYSTEM_ID_TO", null)]
        public void TestSpineAccreditedSystemIdTo(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.SpineAccreditedSystemIdTo, value);
        }
        
        [DataTestMethod]
        [DataRow("PDS_QUERY_FROM_ADDRESS", "test")]
        [DataRow("PDS_QUERY_FROM_ADDRESS", "")]
        [DataRow("PDS_QUERY_FROM_ADDRESS", null)]
        public void TestPdsQueryFromAddress(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.PdsQueryFromAddress, value);
        }
        
        [DataTestMethod]
        [DataRow("PDS_QUERY_TO", "test")]
        [DataRow("PDS_QUERY_TO", "")]
        [DataRow("PDS_QUERY_TO", null)]
        public void TestPdsQueryTo(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.PdsQueryTo, value);
        }
        
        [DataTestMethod]
        [DataRow("PART_SDS_ROLE_ID", "test")]
        [DataRow("PART_SDS_ROLE_ID", "")]
        [DataRow("PART_SDS_ROLE_ID", null)]
        public void TestPartSdsRoleId(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.PartSdsRoleId, value);
        }
        
        [DataTestMethod]
        [DataRow("SDS_USER_ID", "test")]
        [DataRow("SDS_USER_ID", "")]
        [DataRow("SDS_USER_ID", null)]
        public void TestSdsUserId(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.SdsUserId, value);
        }
        
        [DataTestMethod]
        [DataRow("PERSON_SDS_ROLE_ID", "test")]
        [DataRow("PERSON_SDS_ROLE_ID", "")]
        [DataRow("PERSON_SDS_ROLE_ID", null)]
        public void TestPersonSdsRoleId(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.PersonSdsRoleId, value);
        }
        
        [TestMethod]
        public void TestArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds()
        {
            Setup("DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS", "10");
            Assert.AreEqual(_nominatedPharmacyConfig.ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds, 10);
        }
        
        [DataTestMethod]
        [DataRow("DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS", "")]
        [DataRow("DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS", null)]
        public void TestArtificialDelayAfterNominatedPharmacyUpdateInMillisecondsDefault(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds, 0);
        }
        
        [TestMethod]
        public void TestGuid()
        {
            _nominatedPharmacyConfig = new NominatedPharmacyConfig(_mockConfiguration.Object, _mockLogger.Object);
            Assert.IsNotNull(_nominatedPharmacyConfig.MessageId);
        }
        
        [DataTestMethod]
        [DataRow("NOMINATED_PHARMACY_URL", "")]
        [DataRow("NOMINATED_PHARMACY_URL", null)]        
        public void TestBaseUrlNull(string key, string value)
        {
            Setup(key, value);
            Assert.AreEqual(_nominatedPharmacyConfig.BaseUrl, null);
        }
        
        [TestMethod]
        public void TestBaseUrl()
        {
            Setup("NOMINATED_PHARMACY_URL", "http://google.com");
            Assert.AreEqual(_nominatedPharmacyConfig.BaseUrl, new Uri("http://google.com"));
        }
        
        private void Setup(string key, string value)
        {
            _mockConfiguration.SetupGet(x => x[key]).Returns(value);
            _nominatedPharmacyConfig = new NominatedPharmacyConfig(_mockConfiguration.Object, _mockLogger.Object);
        }

    }
}