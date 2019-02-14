using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.DependencyInjection;

namespace NHSOnline.Backend.Support.UnitTests.DependencyInjection
{
    [TestClass]
    public class SupplierServiceConfigurationModuleTests
    {
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }
        
        /// <summary>
        /// This test is for the benefit of future developers on the project.
        /// If they add a new supplier to the Supplier enumeration, it will nudge them to also add an entry into
        ///  SupplierServiceConfigurationModule.SupplierEnabledSettings dictionary (and hence also create
        ///  an env var to enable/disable that Supplier).
        /// </summary>
        [TestMethod]
        public void SupplierEnabledEnvironmentVariables_ContainsAnEntryForEverySupplier()
        {
            var suppliers = Enum.GetValues(typeof(Supplier)).Cast<Supplier>().Where(x => x!=Supplier.Unknown);

            var missingSuppliers = new List<Supplier>();
            
            foreach (var supplier in suppliers)
            {
                if (!EmisSupplierConfigurationModuleDouble.SupplierEnabledSettings.ContainsKey(supplier))
                {
                    missingSuppliers.Add(supplier);
                }
            }

            if (missingSuppliers.Any())
            {
                const string errorMessage =
                    "An entry should exist in SupplierServiceConfigurationModule.SupplierEnabledSettings for each Supplier";
                Assert.Fail(errorMessage);
            }
        }
        
        [TestMethod]
        public void IsEnabled_NoEntryInSupplierEnabledSettingsForSupplier_ReturnsFalse()
        {
            // Arrange
            var systemUnderTest = _fixture.Create<UnknownSupplierConfigurationModuleDouble>();
            var mockConfiguration = new Mock<IConfiguration>();
            
            // Act
            var isEnabled = systemUnderTest.IsEnabled(mockConfiguration.Object);

            // Assert
            isEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void IsEnabled_SupplierEnabledConfigSettingFalse_ReturnsFalse()
        {
            // Arrange
            var mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            mockConfiguration.Setup(x => x["GP_PROVIDER_ENABLED_EMIS"]).Returns("false");

            var systemUnderTest = _fixture.Create<EmisSupplierConfigurationModuleDouble>();
            
            // Act
            var isEnabled = systemUnderTest.IsEnabled(mockConfiguration.Object);

            // Assert
            isEnabled.Should().BeFalse();
        }
        
        [TestMethod]
        public void IsEnabled_SupplierEnabledConfigSettingTrue_ReturnsTrue()
        {
            // Arrange
            var mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            mockConfiguration.Setup(x => x["GP_PROVIDER_ENABLED_EMIS"]).Returns("true");

            var systemUnderTest = _fixture.Create<EmisSupplierConfigurationModuleDouble>();
            
            // Act
            var isEnabled = systemUnderTest.IsEnabled(mockConfiguration.Object);

            // Assert
            isEnabled.Should().BeTrue();
        }
        
        [TestMethod]
        public void IsEnabled_SupplierEnabledConfigSettingMissing_ReturnsFalse()
        {
            // Arrange
            var mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            mockConfiguration.Setup(x => x["GP_PROVIDER_ENABLED_EMIS"]).Returns((string)null);

            var systemUnderTest = _fixture.Create<EmisSupplierConfigurationModuleDouble>();
            
            // Act
            var isEnabled = systemUnderTest.IsEnabled(mockConfiguration.Object);

            // Assert
            isEnabled.Should().BeFalse();
        }
        
        [TestMethod]
        public void IsEnabled_SupplierEnabledConfigSettingNotABoolean_ReturnsFalse()
        {
            // Arrange
            var mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            mockConfiguration.Setup(x => x["GP_PROVIDER_ENABLED_EMIS"]).Returns("cabbages");

            var systemUnderTest = _fixture.Create<EmisSupplierConfigurationModuleDouble>();
            
            // Act
            var isEnabled = systemUnderTest.IsEnabled(mockConfiguration.Object);

            // Assert
            isEnabled.Should().BeFalse();
        }

        private class EmisSupplierConfigurationModuleDouble : SupplierServiceConfigurationModule
        {
            protected override Supplier Supplier => Supplier.Emis;

            public EmisSupplierConfigurationModuleDouble(ILoggerFactory loggerFactory) : base(loggerFactory)
            {
            }
        }
        
        private class UnknownSupplierConfigurationModuleDouble : SupplierServiceConfigurationModule
        {
            protected override Supplier Supplier => Supplier.Unknown;

            public UnknownSupplierConfigurationModuleDouble(ILoggerFactory loggerFactory) : base(loggerFactory)
            {
            }
        }
    }
}