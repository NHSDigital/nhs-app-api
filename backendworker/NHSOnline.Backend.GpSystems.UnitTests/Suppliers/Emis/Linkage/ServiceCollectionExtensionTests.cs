using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Linkage
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckEmisServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisLinkageServices();
            CheckRegisteredEmisLinkageService(services);
        }

        public static void CheckRegisteredEmisLinkageService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var emisLinkageService = new ServiceDescriptor(typeof(EmisLinkageService), 
                typeof(EmisLinkageService), ServiceLifetime.Transient);
            var emisLinkageMapper = new ServiceDescriptor(typeof(IEmisLinkageMapper), 
                typeof(EmisLinkageMapper), ServiceLifetime.Transient);
            var emisLinkageRqstValidationService = new ServiceDescriptor(typeof(EmisLinkageRequestValidationService), 
                typeof(EmisLinkageRequestValidationService), ServiceLifetime.Singleton);

            registeredServices.Should().ContainEquivalentOf(emisLinkageService);
            registeredServices.Should().ContainEquivalentOf(emisLinkageMapper);
            registeredServices.Should().ContainEquivalentOf(emisLinkageRqstValidationService);
        }
    }
}