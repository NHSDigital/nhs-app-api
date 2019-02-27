using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Demographics
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionDemographicsServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionDemographicsServices();

            CheckRegisteredVisionDemographicsServices(services);
        }

        public static void CheckRegisteredVisionDemographicsServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            
            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(VisionDemographicsService), typeof(VisionDemographicsService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionDemographicsMapper), typeof(VisionDemographicsMapper), ServiceLifetime.Transient),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
