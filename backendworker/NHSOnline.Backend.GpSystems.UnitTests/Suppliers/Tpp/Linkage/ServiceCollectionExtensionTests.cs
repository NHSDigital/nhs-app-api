using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckTppServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppLinkageServices();            
            CheckRegisteredTppLinkageServices(services);
        }

        public static void CheckRegisteredTppLinkageServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();
            
            var tppLinkageService = new ServiceDescriptor(typeof(TppLinkageService),
                typeof(TppLinkageService), ServiceLifetime.Transient);
            var tppLinkageRequestValidationService = new ServiceDescriptor(typeof(TppLinkageValidationService),
                typeof(TppLinkageValidationService), ServiceLifetime.Singleton);
            var tppLinkageMapper = new ServiceDescriptor(typeof(ITppLinkageMapper),
                typeof(TppLinkageMapper), ServiceLifetime.Transient);
            
            registeredService.Should().ContainEquivalentOf(tppLinkageService);
            registeredService.Should().ContainEquivalentOf(tppLinkageRequestValidationService);
            registeredService.Should().ContainEquivalentOf(tppLinkageMapper);           
        }
    }
}