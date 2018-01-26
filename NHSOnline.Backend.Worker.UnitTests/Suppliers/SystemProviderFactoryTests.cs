using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Suppliers;
using NHSOnline.Backend.Worker.Suppliers.Emis;

namespace NHSOnline.Backend.Worker.UnitTests.Suppliers
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

            var serviceProvider = serviceCollection.BuildServiceProvider();
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
            Assert.ThrowsException<UnknownSupplierException>(() => _systemProviderFactory.CreateSystemProvider((SupplierEnum)(-1)));
        }
    }
}
