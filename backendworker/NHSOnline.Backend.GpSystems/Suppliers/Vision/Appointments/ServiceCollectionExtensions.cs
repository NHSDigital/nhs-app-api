using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionAppointmentsServices(this IServiceCollection services)
        {
            services.AddTransient<IBookedAppointmentsResponseMapper, BookedAppointmentsResponseMapper>();
            services.AddTransient<IBookedAppointmentMapper, BookedAppointmentMapper>();
            services.AddTransient<IAvailableAppointmentsMapper, AvailableAppointmentsMapper>();
            services.AddTransient<IAvailableAppointmentsResponseMapper, AvailableAppointmentsResponseMapper>();
            services.AddTransient<VisionAppointmentSlotsService>();
            services.AddTransient<ICancellationReasonMapper, CancellationReasonMapper>();
            services.AddTransient<VisionAppointmentsService>();
            services.AddTransient<VisionAppointmentsBookingService>();
            services.AddTransient<VisionAppointmentsRetrievalService>();
            services.AddTransient<VisionAppointmentsCancellationService>();

            return services;
        }
    }
}