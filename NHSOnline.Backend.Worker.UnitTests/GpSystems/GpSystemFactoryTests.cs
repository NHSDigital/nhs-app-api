using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;

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

            var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
            _gpSystemFactory = new GpSystemFactory(serviceProvider);
        }

        [TestMethod]
        public void CreateGpSystem_ReturnsAnEmisGpSystem_WhenTheSupplierIsEmis()
        {
            var result = _gpSystemFactory.CreateGpSystem(SupplierEnum.Emis);

            Assert.IsInstanceOfType(result, typeof(EmisGpSystem));
        }

        [TestMethod]
        public void CreateGpSystem_ThrowsAnUnknownSystemException_WhenTheSupplierNameIsUnknown()
        {
            Assert.ThrowsException<UnknownSupplierException>(() =>
                _gpSystemFactory.CreateGpSystem((SupplierEnum) (-1)));
        }
    }
}