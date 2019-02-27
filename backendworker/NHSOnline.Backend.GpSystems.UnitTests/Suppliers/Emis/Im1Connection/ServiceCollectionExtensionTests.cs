using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Im1Connection
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckEmisServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisIm1ConnectionServices();
            CheckRegisteredEmisIm1ConnectionService(services);
        }

        public static void CheckRegisteredEmisIm1ConnectionService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var emisIm1ConnectionService = new ServiceDescriptor(typeof(EmisIm1ConnectionService), 
                typeof(EmisIm1ConnectionService), ServiceLifetime.Transient);

            registeredServices.Should().ContainEquivalentOf(emisIm1ConnectionService);
        }
    }
}