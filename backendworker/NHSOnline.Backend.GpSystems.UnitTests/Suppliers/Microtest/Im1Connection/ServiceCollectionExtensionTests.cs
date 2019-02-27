using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Im1Connection
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckMicrotestServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterMicrotestIm1ConnectionServices();            
            CheckRegisteredMicrotestIm1ConnectionServices(services);
        }

        public static void CheckRegisteredMicrotestIm1ConnectionServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();            
            var microtestIm1ConnectionService = new ServiceDescriptor(typeof(MicrotestIm1ConnectionService),
                typeof(MicrotestIm1ConnectionService), ServiceLifetime.Transient);
            
            registeredService.Should().ContainEquivalentOf(microtestIm1ConnectionService);
        }
    }
}