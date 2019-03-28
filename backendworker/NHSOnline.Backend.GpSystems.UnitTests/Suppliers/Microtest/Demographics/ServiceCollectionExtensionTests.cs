using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Demographics
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckMicrotestServiceCollectionExtensions()
        {
            var services = new ServiceCollection();
            services.RegisterMicrotestDemographicsServices();

            CheckRegisteredMicrotestDemographicsService(services);
        }

        public static void CheckRegisteredMicrotestDemographicsService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var microtestDemographicService = new ServiceDescriptor(typeof(IMicrotestDemographicsService),
                typeof(MicrotestDemographicsService), ServiceLifetime.Transient);
            var microtestDemographicMapper = new ServiceDescriptor(typeof(IMicrotestDemographicsMapper),
                typeof(MicrotestDemographicsMapper), ServiceLifetime.Transient);

            registeredServices.Should().ContainEquivalentOf(microtestDemographicService);
            registeredServices.Should().ContainEquivalentOf(microtestDemographicMapper);
        }
    }
}