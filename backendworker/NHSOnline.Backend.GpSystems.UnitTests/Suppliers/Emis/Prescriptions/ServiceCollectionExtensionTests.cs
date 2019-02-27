using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Prescriptions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckEmisPrescriptionServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisPrescriptionsServices();
            CheckRegisteredEmisPrescriptionServices(services);
        }
        
        public static void CheckRegisteredEmisPrescriptionServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var emisCourseService = new ServiceDescriptor(typeof(EmisCourseService),
                typeof(EmisCourseService), ServiceLifetime.Transient);
            var emisPrescriptionService = new ServiceDescriptor(typeof(EmisPrescriptionService),
                typeof(EmisPrescriptionService), ServiceLifetime.Transient);
            var emisPrescriptionRqstValidationService = new ServiceDescriptor(typeof(EmisPrescriptionRequestValidationService),
                typeof(EmisPrescriptionRequestValidationService), ServiceLifetime.Transient);
            var emisPrescriptionMapper = new ServiceDescriptor(typeof(IEmisPrescriptionMapper),
                typeof(EmisPrescriptionMapper), ServiceLifetime.Transient);
            
            registeredServices.Should().ContainEquivalentOf(emisCourseService);
            registeredServices.Should().ContainEquivalentOf(emisPrescriptionService);
            registeredServices.Should().ContainEquivalentOf(emisPrescriptionRqstValidationService);
            registeredServices.Should().ContainEquivalentOf(emisPrescriptionMapper);
        }
    }
}