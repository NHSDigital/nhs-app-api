using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Demographics
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckEmisServiceCollectionExtensions()
        {
            var services = new ServiceCollection();
            services.RegisterEmisDemographicsServices();

            CheckRegisteredEmisDemographicsService(services);
        }

        public static void CheckRegisteredEmisDemographicsService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var emisDemographicService = new ServiceDescriptor(typeof(EmisDemographicsService), 
                typeof(EmisDemographicsService), ServiceLifetime.Transient);
            var emisDemographicMapper = new ServiceDescriptor(typeof(IEmisDemographicsMapper), 
                typeof(EmisDemographicsMapper), ServiceLifetime.Transient);

            registeredServices.Should().ContainEquivalentOf(emisDemographicService);
            registeredServices.Should().ContainEquivalentOf(emisDemographicMapper);
        }
    }
}