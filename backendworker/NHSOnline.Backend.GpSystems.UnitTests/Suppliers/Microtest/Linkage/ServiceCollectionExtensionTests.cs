using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Linkage
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckMicrotestServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterMicrotestLinkageServices();           
            CheckRegisteredMicrotestLinkageServices(services);
        }

        public static void CheckRegisteredMicrotestLinkageServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();            
            var microtestLinkageService = new ServiceDescriptor(typeof(MicrotestLinkageService),
                typeof(MicrotestLinkageService), ServiceLifetime.Transient);
            var microtestLinkageRequestValidationService = new ServiceDescriptor(typeof(MicrotestLinkageValidationService),
                typeof(MicrotestLinkageValidationService), ServiceLifetime.Singleton);
            
            registeredService.Should().ContainEquivalentOf(microtestLinkageService);
            registeredService.Should().ContainEquivalentOf(microtestLinkageRequestValidationService);
        }
    }
}