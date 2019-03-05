using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckRegisteredPfsServicesForAllSuppliers()
        {
            // Arrange
            var services = new ServiceCollection();
            var config = new EnableGpSupplierConfiguration
            {
                EnableTpp = true,
                EnableEmis = true,
                EnableMicrotest = true,
                EnableVision = true
            };

            // Act
            services.RegisterPfsGpSystemsServices(config);

            // Assert
            CheckBaseRegisteredServices(services);
            CheckAllPfsRegisteredServices(services);
        }
        
        [TestMethod]
        public void CheckRegisteredCidServicesForAllSuppliers()
        {
            // Arrange
            var services = new ServiceCollection();
            var config = new EnableGpSupplierConfiguration
            {
                EnableTpp = true,
                EnableEmis = true,
                EnableMicrotest = true,
                EnableVision = true
            };

            // Act
            services.RegisterCidGpSystemsServices(config);

            // Assert
            CheckBaseRegisteredServices(services);
            CheckAllCidRegisteredServices(services);
        }

        [TestMethod]
        public void CheckGpServicesAreNotRegistered()
        {
            // Arrange
            var services = new ServiceCollection();

            var config = new EnableGpSupplierConfiguration {
                EnableTpp = false,
                EnableEmis = false,
                EnableMicrotest = false,
                EnableVision = false
            };

            // Act
            services.RegisterPfsGpSystemsServices(config);

            // Assert
            CheckBaseRegisteredServices(services);
        }

        private static void CheckBaseRegisteredServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(IHtmlSanitizer), typeof(HtmlSanitizer), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IGpSystemFactory), typeof(GpSystemFactory), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IIm1CacheService), typeof(Im1CacheService), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IIm1CacheKeyGenerator), typeof(Im1CacheKeyGenerator), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IAppointmentCancellationReasonScraper), typeof(AppointmentCancellationReasonScraper), ServiceLifetime.Singleton)
            };

            foreach (var dependency in dependencies) {
                serviceCollection.Should().ContainEquivalentOf(dependency);
            }
        }

        private static void CheckAllPfsRegisteredServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            Suppliers.Emis.ServiceCollectionExtensionsTests.CheckAllEmisPfsRegisteredServices(serviceCollection);
            Suppliers.Microtest.ServiceCollectionExtensionTests.CheckAllMicrotestPfsRegisteredServices(serviceCollection);
            Suppliers.Tpp.ServiceCollectionExtensionTests.CheckAllTppPfsRegisteredServices(serviceCollection);
            Suppliers.Vision.ServiceCollectionExtensionsTests.CheckAllVisionPfsRegisteredServices(serviceCollection);
        }
        
        private static void CheckAllCidRegisteredServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            Suppliers.Emis.ServiceCollectionExtensionsTests.CheckAllEmisCidRegisteredServices(serviceCollection);
            Suppliers.Microtest.ServiceCollectionExtensionTests.CheckAllMicrotestCidRegisteredServices(serviceCollection);
            Suppliers.Tpp.ServiceCollectionExtensionTests.CheckAllTppCidRegisteredServices(serviceCollection);
            Suppliers.Vision.ServiceCollectionExtensionsTests.CheckAllVisionCidRegisteredServices(serviceCollection);
        }
    }
}