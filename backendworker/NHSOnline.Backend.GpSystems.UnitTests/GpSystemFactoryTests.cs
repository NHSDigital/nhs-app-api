using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests
{
    [TestClass]
    public class GpSystemFactoryTests
    {
        private GpSystemFactory _gpSystemFactory;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            var logger = _fixture.Create<ILogger<GpSystemFactory>>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IGpSystem, EmisGpSystem>();
            serviceCollection.AddSingleton<IGpSystem, TppGpSystem>();

            var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
            _gpSystemFactory = new GpSystemFactory(serviceProvider, logger);
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
    }
}