using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckTppServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppAppointmentsServices();
            CheckRegisteredTppAppointmentServices(services);
        }

        public static void CheckRegisteredTppAppointmentServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();
            var tppAppointmentService = new ServiceDescriptor(typeof(TppAppointmentsService),
                typeof(TppAppointmentsService), ServiceLifetime.Transient);
            var tppAppointmentSlotService = new ServiceDescriptor(typeof(TppAppointmentSlotsService),
                typeof(TppAppointmentSlotsService), ServiceLifetime.Transient);

            var tppAppointmentRetrievalService = new ServiceDescriptor(typeof(TppAppointmentsRetrievalService),
                typeof(TppAppointmentsRetrievalService), ServiceLifetime.Transient);
            var tppAppointmentBookingService = new ServiceDescriptor(typeof(TppAppointmentsBookingService),
                typeof(TppAppointmentsBookingService), ServiceLifetime.Transient);
            var tppAppointmentCancellationService = new ServiceDescriptor(typeof(TppAppointmentsCancellationService),
                typeof(TppAppointmentsCancellationService), ServiceLifetime.Transient);

            var tppListSlotReplyMapper = new ServiceDescriptor(typeof(IAppointmentSlotsMapper),
                typeof(AppointmentSlotsMapper), ServiceLifetime.Transient);
            var tppSessionMapper = new ServiceDescriptor(typeof(ISessionMapper),
                typeof(SessionMapper), ServiceLifetime.Transient);
            var tppAppointmentsReplyMapper = new ServiceDescriptor(typeof(IAppointmentsReplyMapper),
                typeof(AppointmentsReplyMapper), ServiceLifetime.Transient);
            var tppAppointmentMapper = new ServiceDescriptor(typeof(IAppointmentMapper),
                typeof(AppointmentMapper), ServiceLifetime.Transient);
            var tppAppointmentsResultBuilder = new ServiceDescriptor(typeof(IAppointmentsResultBuilder),
                typeof(TppAppointmentsResultBuilder), ServiceLifetime.Singleton);

            registeredService.Should().ContainEquivalentOf(tppAppointmentService);
            registeredService.Should().ContainEquivalentOf(tppAppointmentSlotService);

            registeredService.Should().ContainEquivalentOf(tppAppointmentRetrievalService);
            registeredService.Should().ContainEquivalentOf(tppAppointmentBookingService);
            registeredService.Should().ContainEquivalentOf(tppAppointmentCancellationService);

            registeredService.Should().ContainEquivalentOf(tppListSlotReplyMapper);
            registeredService.Should().ContainEquivalentOf(tppSessionMapper);
            registeredService.Should().ContainEquivalentOf(tppAppointmentsReplyMapper);
            registeredService.Should().ContainEquivalentOf(tppAppointmentMapper);
            registeredService.Should().ContainEquivalentOf(tppAppointmentsResultBuilder);
        }
    }
}