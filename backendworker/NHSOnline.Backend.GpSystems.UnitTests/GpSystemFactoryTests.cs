using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests
{
    [TestClass]
    public class GpSystemFactoryTests
    {
        private GpSystemFactory _gpSystemFactory;
        private Mock<IOdsCodeLookup> _mockOdsLookup;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockOdsLookup = _fixture.Create<Mock<IOdsCodeLookup>>();
            var logger = _fixture.Create<ILogger<GpSystemFactory>>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IGpSystem, EmisGpSystem>();
            serviceCollection.AddSingleton<IGpSystem, TppGpSystem>();

            var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
            _gpSystemFactory = new GpSystemFactory(serviceProvider, _mockOdsLookup.Object, logger);
        }

        [TestMethod]
        public void CreateGpSystem_ReturnsAnEmisGpSystem_WhenTheSupplierIsEmis()
        {
            // Act
            var gpSystem = _gpSystemFactory
                .CreateGpSystem(Supplier.Emis);
                
            // Assert
            gpSystem.Should().BeOfType<EmisGpSystem>();
        }

        [TestMethod]
        public void CreateGpSystem_ReturnATppGpSystem_WhenTheSupplierIsTpp()
        {
            // Act
            var gpSystem = _gpSystemFactory
                .CreateGpSystem(Supplier.Tpp);
                
            // Assert
            gpSystem.Should().BeOfType<TppGpSystem>();
        }

        [TestMethod]
        public void CreateGpSystem_ThrowsAnUnknownSystemException_WhenTheSupplierNameIsUnknown()
        {
            // Act
            var act = new Action(() => _gpSystemFactory.CreateGpSystem((Supplier) (-1)));
                
            // Assert
            act.Should().Throw<UnknownSupplierException>();
        }

        [TestMethod]
        public void LookupGpSystem__WhenOdsCodeIsValid_ReturnAGpSystem()
        {
            // Arrange
            const string odsCode = "A1234";
            _mockOdsLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.Some(Supplier.Tpp)));

            // Act
            var result = _gpSystemFactory.LookupGpSystem(odsCode);
            
            // Assert
            result.Result.HasValue.Should().BeTrue();
            result.Result.ValueOrFailure().Should().BeOfType<TppGpSystem>();
        }

        [TestMethod]
        public void LookupGpSystem__WhenOdsCodeIsUnknown_ReturnAnOptionOfNone()
        {
            // Arrange
            const string odsCode = "A1234";
            _mockOdsLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.Some(Supplier.Unknown)));

            // Act
            var result = _gpSystemFactory.LookupGpSystem(odsCode);
                
            // Assert
            result.Result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void LookupGpSystem__WhenOdsCodeIsNone_ReturnAnOptionOfNone()
        {
            // Arrange
            const string odsCode = "A1234";
            _mockOdsLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.None<Supplier>()));

            // Act
            var result = _gpSystemFactory.LookupGpSystem(odsCode);
                
            // Assert
            result.Result.HasValue.Should().BeFalse();
        }
    }
}