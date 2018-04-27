using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.UnitTests.Router
{
    [TestClass]
    public class SystemProviderFactoryTests
    {
        private SystemProviderFactory _systemProviderFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<EmisSystemProvider, EmisSystemProvider>();
            serviceCollection.AddSingleton(new Mock<IEmisClient>().Object);
            serviceCollection.AddSingleton(new Mock<IEmisPrescriptionMapper>().Object);

            var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
            _systemProviderFactory = new SystemProviderFactory(serviceProvider);
        }

        [TestMethod]
        public void CreateSystemProvider_ReturnsAnEmisSystemProvider_WhenTheSupplierIsEmis()
        {
            var result = _systemProviderFactory.CreateSystemProvider(SupplierEnum.Emis);

            Assert.IsInstanceOfType(result, typeof(EmisSystemProvider));
        }

        [TestMethod]
        public void CreateSystemProvider_ThrowsAnUnknownSystemException_WhenTheSupplierNameIsUnknown()
        {
            Assert.ThrowsException<UnknownSupplierException>(() =>
                _systemProviderFactory.CreateSystemProvider((SupplierEnum) (-1)));
        }
    }
}