using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Demographics
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckTppServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppDemographicsServices();            
            CheckRegisteredTppDemographicServices(services);
        }

        public static void CheckRegisteredTppDemographicServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();
            var tppDemographicService = new ServiceDescriptor(typeof(TppDemographicsService),
                typeof(TppDemographicsService), ServiceLifetime.Transient);
            var tppDemographicMapper = new ServiceDescriptor(typeof(ITppDemographicsMapper),
                typeof(TppDemographicsMapper), ServiceLifetime.Transient);
        
            registeredService.Should().ContainEquivalentOf(tppDemographicService);
            registeredService.Should().ContainEquivalentOf(tppDemographicMapper);    
        }
    }
}