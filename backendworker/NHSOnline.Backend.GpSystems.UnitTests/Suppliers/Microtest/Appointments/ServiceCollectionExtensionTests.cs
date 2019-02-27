using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckMicrotestServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterMicrotestAppointmentsServices();
            CheckRegisteredMicrotestAppointmentServices(services);
        }

        public static void CheckRegisteredMicrotestAppointmentServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();
            var microtestAppointmentSlotsService = new ServiceDescriptor(typeof(MicrotestAppointmentSlotsService),
                typeof(MicrotestAppointmentSlotsService), ServiceLifetime.Transient);
            var microtestAppointmentService = new ServiceDescriptor(typeof(MicrotestAppointmentsService),
                typeof(MicrotestAppointmentsService), ServiceLifetime.Transient);
            var microtestAppointmentSlotResponseMapper = new ServiceDescriptor(typeof(IAppointmentSlotsResponseMapper),
                typeof(AppointmentSlotsResponseMapper), ServiceLifetime.Transient);

            registeredService.Should().ContainEquivalentOf(microtestAppointmentSlotsService);
            registeredService.Should().ContainEquivalentOf(microtestAppointmentService);
            registeredService.Should().ContainEquivalentOf(microtestAppointmentSlotResponseMapper);
        }
    }
}