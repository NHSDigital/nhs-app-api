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
            var tppAppointmemntService = new ServiceDescriptor(typeof(TppAppointmentsService),
                typeof(TppAppointmentsService), ServiceLifetime.Transient);
            var tppAppointmemntSlotService = new ServiceDescriptor(typeof(TppAppointmentSlotsService),
                typeof(TppAppointmentSlotsService), ServiceLifetime.Transient);

            var tppAppointmemntRetrievalService = new ServiceDescriptor(typeof(TppAppointmentsRetrievalService),
                typeof(TppAppointmentsRetrievalService), ServiceLifetime.Transient);
            var tppAppointmemntBookingService = new ServiceDescriptor(typeof(TppAppointmentsBookingService),
                typeof(TppAppointmentsBookingService), ServiceLifetime.Transient);
            var tppAppointmemntCancellationService = new ServiceDescriptor(typeof(TppAppointmentsCancellationService),
                typeof(TppAppointmentsCancellationService), ServiceLifetime.Transient);

            var tppListSlotReplyMapper = new ServiceDescriptor(typeof(IListSlotsReplyMapper),
                typeof(ListSlotsReplyMapper), ServiceLifetime.Transient);
            var tppSessionMapper = new ServiceDescriptor(typeof(ISessionMapper),
                typeof(SessionMapper), ServiceLifetime.Transient);
            var tppAppointmentSlotResultBuilderMapper = new ServiceDescriptor(typeof(IAppointmentSlotResultBuilder),
                typeof(TppAppointmentSlotsResultBuilder), ServiceLifetime.Singleton);
            var tppAppointmentsReplyMapper = new ServiceDescriptor(typeof(IAppointmentsReplyMapper),
                typeof(AppointmentsReplyMapper), ServiceLifetime.Transient);
            var tppAppointmentMapper = new ServiceDescriptor(typeof(IAppointmentMapper),
                typeof(AppointmentMapper), ServiceLifetime.Transient);
            var tppAppointmentsResultBuilder = new ServiceDescriptor(typeof(IAppointmentsResultBuilder),
                typeof(TppAppointmentsResultBuilder), ServiceLifetime.Singleton);


            registeredService.Should().ContainEquivalentOf(tppAppointmemntService);
            registeredService.Should().ContainEquivalentOf(tppAppointmemntSlotService);

            registeredService.Should().ContainEquivalentOf(tppAppointmemntRetrievalService);
            registeredService.Should().ContainEquivalentOf(tppAppointmemntBookingService);
            registeredService.Should().ContainEquivalentOf(tppAppointmemntCancellationService);

            registeredService.Should().ContainEquivalentOf(tppListSlotReplyMapper);
            registeredService.Should().ContainEquivalentOf(tppSessionMapper);
            registeredService.Should().ContainEquivalentOf(tppAppointmentSlotResultBuilderMapper);
            registeredService.Should().ContainEquivalentOf(tppAppointmentsReplyMapper);
            registeredService.Should().ContainEquivalentOf(tppAppointmentMapper);
            registeredService.Should().ContainEquivalentOf(tppAppointmentsResultBuilder);
        }
    }
}