using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckTppServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppPrescriptionsServices();
            CheckRegisteredTppPrescriptionServices(services);
        }

        public static void CheckRegisteredTppPrescriptionServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();

            var tppCourseService = new ServiceDescriptor(typeof(TppCourseService),
                typeof(TppCourseService), ServiceLifetime.Transient);
            var tppPrescriptionService = new ServiceDescriptor(typeof(TppPrescriptionService),
                typeof(TppPrescriptionService), ServiceLifetime.Transient);
            var tppPrescriptionRequestValidationService = new ServiceDescriptor(
                typeof(TppPrescriptionValidationService),
                typeof(TppPrescriptionValidationService), ServiceLifetime.Transient);

            var tppCourseMapper = new ServiceDescriptor(typeof(ITppCourseMapper),
                typeof(TppCourseMapper), ServiceLifetime.Transient);
            var tppPrescriptionMapper = new ServiceDescriptor(typeof(ITppPrescriptionMapper),
                typeof(TppPrescriptionMapper), ServiceLifetime.Transient);

            registeredService.Should().ContainEquivalentOf(tppCourseService);
            registeredService.Should().ContainEquivalentOf(tppPrescriptionService);
            registeredService.Should().ContainEquivalentOf(tppPrescriptionRequestValidationService);

            registeredService.Should().ContainEquivalentOf(tppCourseMapper);
            registeredService.Should().ContainEquivalentOf(tppPrescriptionMapper);
        }
    }
}