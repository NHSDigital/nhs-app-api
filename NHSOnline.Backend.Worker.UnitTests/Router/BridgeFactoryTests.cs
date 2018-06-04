using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Demographics;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.UnitTests.Router
{
    [TestClass]
    public class BridgeFactoryTests
    {
        private BridgeFactory _bridgeFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<EmisBridge, EmisBridge>();
            serviceCollection.AddSingleton(new Mock<IEmisClient>().Object);
            serviceCollection.AddSingleton(new Mock<IEmisPrescriptionMapper>().Object);
            serviceCollection.AddSingleton(new Mock<IEmisDemographicsMapper>().Object);
            serviceCollection.AddSingleton(new Mock<TimeZoneInfoProvider>().Object);
            serviceCollection.AddSingleton(new Mock<IDateTimeOffsetProvider>().Object);
            serviceCollection.AddSingleton(new Mock<IConfiguration>().Object);
            serviceCollection.AddSingleton<IBridge, EmisBridge>();

            var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
            _bridgeFactory = new BridgeFactory(serviceProvider);
        }

        [TestMethod]
        public void CreateBridge_ReturnsAnEmisBridge_WhenTheSupplierIsEmis()
        {
            var result = _bridgeFactory.CreateBridge(SupplierEnum.Emis);

            Assert.IsInstanceOfType(result, typeof(EmisBridge));
        }

        [TestMethod]
        public void CreateBridge_ThrowsAnUnknownSystemException_WhenTheSupplierNameIsUnknown()
        {
            Assert.ThrowsException<UnknownSupplierException>(() =>
                _bridgeFactory.CreateBridge((SupplierEnum) (-1)));
        }
    }
}