using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Session
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckEmisPfsSessionServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisPfsSessionServices();
            CheckRegisteredEmisPfsSessionService(services);
        }

        [TestMethod]
        public void CheckEmisCidSessionServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisCidSessionServices();
            CheckRegisteredEmisCidSessionService(services);
        }

        public static void CheckRegisteredEmisPfsSessionService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var emisSessionService = new ServiceDescriptor(typeof(IEmisSessionService), 
                typeof(EmisSessionService), ServiceLifetime.Transient);
            var emisSessionExtendService = new ServiceDescriptor(typeof(EmisSessionExtendService),
                typeof(EmisSessionExtendService), ServiceLifetime.Transient);

            registeredServices.Should().ContainEquivalentOf(emisSessionService);
            registeredServices.Should().ContainEquivalentOf(emisSessionExtendService);
        }

        public static void CheckRegisteredEmisCidSessionService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var emisSessionService = new ServiceDescriptor(typeof(IEmisSessionService),
                typeof(EmisSessionService), ServiceLifetime.Transient);

            registeredServices.Should().ContainEquivalentOf(emisSessionService);
        }
    }
}