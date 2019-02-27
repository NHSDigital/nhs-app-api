using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Im1Connection
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckTppServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppIm1ConnectionServices();            
            CheckRegisteredTppIm1Services(services);
        }

        public static void CheckRegisteredTppIm1Services(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();            
            var tppIm1ConnectionService = new ServiceDescriptor(typeof(TppIm1ConnectionService),
                typeof(TppIm1ConnectionService), ServiceLifetime.Transient);
                        
            registeredService.Should().ContainEquivalentOf(tppIm1ConnectionService);            
        }
    }
}