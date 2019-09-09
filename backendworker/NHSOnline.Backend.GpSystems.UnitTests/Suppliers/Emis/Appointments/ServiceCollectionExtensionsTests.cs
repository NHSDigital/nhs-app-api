using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {        
        [TestMethod]
        public void CheckEmisServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisAppointmentsServices();                        
            CheckRegisteredEmisAppointmentServices(services);
        }

        public static void CheckRegisteredEmisAppointmentServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var emisAppointmentService = new ServiceDescriptor(typeof(EmisAppointmentsService), 
                typeof(EmisAppointmentsService), ServiceLifetime.Transient);
            var emisAppointmentSlotService = new ServiceDescriptor(typeof(EmisAppointmentSlotsService), 
                typeof(EmisAppointmentSlotsService), ServiceLifetime.Transient);
            var emisAppointmentRetrievalService = new ServiceDescriptor(typeof(EmisAppointmentsRetrievalService), 
                typeof(EmisAppointmentsRetrievalService), ServiceLifetime.Transient);
            var emisAppointmentBookingService = new ServiceDescriptor(typeof(EmisAppointmentsBookingService), 
                typeof(EmisAppointmentsBookingService), ServiceLifetime.Transient);
            var emisAppointmentCancellationService = new ServiceDescriptor(typeof(EmisAppointmentsCancellationService), 
                typeof(EmisAppointmentsCancellationService), ServiceLifetime.Transient);
            var appointmentSlotsResponseMapper = new ServiceDescriptor(typeof(IAppointmentSlotsResponseMapper), 
                typeof(AppointmentSlotsResponseMapper), ServiceLifetime.Transient);
            var appointmentSlotsMapper = new ServiceDescriptor(typeof(IAppointmentSlotsMapper), 
                typeof(AppointmentSlotsMapper), ServiceLifetime.Transient);
            var appointmentResponseMapper = new ServiceDescriptor(typeof(IAppointmentsResponseMapper), 
                typeof(AppointmentsResponseMapper), ServiceLifetime.Transient);
            var appointmentMapper = new ServiceDescriptor(typeof(IAppointmentsMapper), 
                typeof(AppointmentsMapper), ServiceLifetime.Transient);
            
            registeredServices.Should().ContainEquivalentOf(emisAppointmentService);
            registeredServices.Should().ContainEquivalentOf(emisAppointmentSlotService);
            registeredServices.Should().ContainEquivalentOf(emisAppointmentRetrievalService);
            registeredServices.Should().ContainEquivalentOf(emisAppointmentBookingService);
            registeredServices.Should().ContainEquivalentOf(emisAppointmentCancellationService);
            registeredServices.Should().ContainEquivalentOf(appointmentSlotsResponseMapper);
            registeredServices.Should().ContainEquivalentOf(appointmentSlotsMapper);
            registeredServices.Should().ContainEquivalentOf(appointmentResponseMapper);
            registeredServices.Should().ContainEquivalentOf(appointmentMapper);
        }    
    }
}
