using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests
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
                .CreateGpSystem(Supplier.Emis)
                .Should()
                .BeOfType<EmisGpSystem>();
        }

        [TestMethod]
        public void CreateGpSystem_ReturnATppGpSystem_WhenTheSupplierIsTpp()
        {
            _gpSystemFactory
                .CreateGpSystem(Supplier.Tpp)
                .Should()
                .BeOfType<TppGpSystem>();
        }

        [TestMethod]
        public void CreateGpSystem_ThrowsAnUnknownSystemException_WhenTheSupplierNameIsUnknown()
        {
            new Action(() => _gpSystemFactory.CreateGpSystem((Supplier) (-1)))
                .Should()
                .Throw<UnknownSupplierException>();
        }
    }
}