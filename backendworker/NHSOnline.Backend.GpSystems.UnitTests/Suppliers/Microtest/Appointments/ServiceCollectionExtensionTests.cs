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
            var microtestAppointmentsService = new ServiceDescriptor(typeof(MicrotestAppointmentsService),
                typeof(MicrotestAppointmentsService), ServiceLifetime.Transient);
            var microtestAppointmentsRetrievalService = new ServiceDescriptor(typeof(MicrotestAppointmentsRetrievalService),
                typeof(MicrotestAppointmentsRetrievalService), ServiceLifetime.Transient);
            var microtestAppointmentsBookingService = new ServiceDescriptor(typeof(MicrotestAppointmentsBookingService),
                typeof(MicrotestAppointmentsBookingService), ServiceLifetime.Transient);
            var microtestAppointmentSlotResponseMapper = new ServiceDescriptor(typeof(IAppointmentSlotsResponseMapper),
                typeof(AppointmentSlotsResponseMapper), ServiceLifetime.Transient);
            var microtestAppointmentsResponseMapper = new ServiceDescriptor(typeof(IAppointmentsResponseMapper),
                typeof(AppointmentsResponseMapper), ServiceLifetime.Transient);

            registeredService.Should().ContainEquivalentOf(microtestAppointmentSlotsService);
            registeredService.Should().ContainEquivalentOf(microtestAppointmentsService);
            registeredService.Should().ContainEquivalentOf(microtestAppointmentsBookingService);
            registeredService.Should().ContainEquivalentOf(microtestAppointmentsRetrievalService);
            registeredService.Should().ContainEquivalentOf(microtestAppointmentSlotResponseMapper);
            registeredService.Should().ContainEquivalentOf(microtestAppointmentsResponseMapper);
        }
    }
}