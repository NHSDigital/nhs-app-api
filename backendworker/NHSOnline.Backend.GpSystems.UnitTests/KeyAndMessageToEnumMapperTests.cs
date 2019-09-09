using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.UnitTests
{
    [TestClass]
    public class KeyAndMessageToEnumMapperTests
    {
        private ILogger<EmisLinkageService> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = fixture.Create<ILoggerFactory>().CreateLogger<EmisLinkageService>();
        }

        private readonly KeyAndMessageToEnumMapper<InternalCode> _mapperUnderTest =
            new KeyAndMessageToEnumMapper<InternalCode>()
                .Add("4031030", "Patient Facing Services API v2 is not enabled at this practice",
                    InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice)
                
                .Add("400", "LinkageKey length outside of valid range",
                    InternalCode.LinkageKeyLengthOutsideOfValidRange)
                
                .Add("400", "length outside of valid range",
                    InternalCode.InputValueLengthOutsideOfValidRange)
                
                .AddMessageToEnum("Registered online user is already linked",
                    InternalCode.UserAlreadyLinked)
                
                .AddMessageToEnum("No registered online user found for given linkage details",
                    InternalCode.NoUserFoundForLinkageDetails)
                
                .AddKeyToEnum("4001552", InternalCode.PatientArchived)
                .AddKeyToEnum("4001108", InternalCode.UserAlreadyLinked);

        [TestMethod]
        [DataRow("4031030", "Patient Facing Services API v2 is not enabled at this practice",
            InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice)]
        [DataRow("400", "One of the parameter length outside of valid range",
            InternalCode.InputValueLengthOutsideOfValidRange)]
        [DataRow("300", "Registered online user is already linked",
            InternalCode.UserAlreadyLinked)]
        [DataRow("4001552", "Something or other", InternalCode.PatientArchived)]
        [DataRow("4001552", null, InternalCode.PatientArchived)]
        [DataRow("4001552", "", InternalCode.PatientArchived)]
        public void Map_Successful(string key, string message, InternalCode expectedOutcome)
        {
            // Act
            var result = _mapperUnderTest.Map(_logger, key, message);

            // Assert
            result.Should().Be(expectedOutcome);
        }

        [TestMethod]
        [DataRow("4031031", "Patient Facing Services API v2 is not enabled at this practice")]
        [DataRow("400", "One of the parameter lengths outside of valid range")]
        [DataRow("400", null)]
        [DataRow("400", "")]
        public void Map_Unsuccessful(string key, string message)
        {
            // Act
            var result = _mapperUnderTest.Map(_logger, key, message);

            // Assert
            result.Should().Be(null);
        }

        [TestMethod]
        public void Map_MultipleMessages_Successful()
        {
            // Arrange
            const string key = "4031030";
            const string message1 = "Patient Facing Services API v2 is not enabled at this practice";
            const string message2 = "Non matching";
            const InternalCode expectedOutcome = InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice;

            // Act
            var result1 = _mapperUnderTest.Map(_logger, key, message1, message2);
            var result2 = _mapperUnderTest.Map(_logger, key, message2, message1);

            // Assert
            result1.Should().Be(expectedOutcome);
            result2.Should().Be(expectedOutcome);
        }

        [TestMethod]
        public void Map_NullKey_ThrowsException()
        {
            // Act
            var act =  new Action(() => _mapperUnderTest.Map(_logger, null, "Something"));
                
            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("key");
        }
    }
}