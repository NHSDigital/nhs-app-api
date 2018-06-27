using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems
{
    [TestClass]
    public class GpSystemFactoryTests
    {
        private GpSystemFactory _gpSystemFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IGpSystem, EmisGpSystem>();
            serviceCollection.AddSingleton<IGpSystem, TppGpSystem>();

            var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
            _gpSystemFactory = new GpSystemFactory(serviceProvider);
        }

        [TestMethod]
        public void CreateGpSystem_ReturnsAnEmisGpSystem_WhenTheSupplierIsEmis()
        {
            _gpSystemFactory
                .CreateGpSystem(SupplierEnum.Emis)
                .Should()
                .BeOfType<EmisGpSystem>();
        }

        [TestMethod]
        public void CreateGpSystem_ReturnATppGpSystem_WhenTheSupplierIsTpp()
        {
            _gpSystemFactory
                .CreateGpSystem(SupplierEnum.Tpp)
                .Should()
                .BeOfType<TppGpSystem>();
        }

        [TestMethod]
        public void CreateGpSystem_ThrowsAnUnknownSystemException_WhenTheSupplierNameIsUnknown()
        {
            new Action(() => _gpSystemFactory.CreateGpSystem((SupplierEnum) (-1)))
                .Should()
                .Throw<UnknownSupplierException>();
        }
    }
}