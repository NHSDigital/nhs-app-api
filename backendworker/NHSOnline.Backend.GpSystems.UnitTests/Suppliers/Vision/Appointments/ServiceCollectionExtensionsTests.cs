using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Appointments
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionAppointmentServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionAppointmentsServices();
            
            CheckRegisteredVisionAppointmentServices(services);
        }

        public static void CheckRegisteredVisionAppointmentServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(IBookedAppointmentsResponseMapper), typeof(BookedAppointmentsResponseMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IBookedAppointmentMapper), typeof(BookedAppointmentMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IAvailableAppointmentsMapper), typeof(AvailableAppointmentsMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionAppointmentSlotsService), typeof(VisionAppointmentSlotsService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(ICancellationReasonMapper), typeof(CancellationReasonMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionAppointmentsService), typeof(VisionAppointmentsService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionAppointmentsBookingService), typeof(VisionAppointmentsBookingService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionAppointmentsRetrievalService), typeof(VisionAppointmentsRetrievalService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionAppointmentsCancellationService), typeof(VisionAppointmentsCancellationService), ServiceLifetime.Transient)
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
