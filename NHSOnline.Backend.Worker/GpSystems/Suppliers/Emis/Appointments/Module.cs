using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAppointmentSlotsResponseMapper, AppointmentSlotsResponseMapper>();
            services.AddTransient<IAppointmentSlotsMapper, AppointmentSlotsMapper>();
            services.AddTransient<IAppointmentsResponseMapper, AppointmentsResponseMapper>();
            services.AddTransient<IAppointmentsMapper, AppointmentsMapper>();
            services.AddTransient<IMapper<SessionHolder, Clinician>, AppointmentClinicianMapper>();
            services.AddTransient<IMapper<Models.Location, Areas.Appointments.Models.Location>, AppointmentLocationMapper>();
            services.AddTransient<IMapper<Models.Session, AppointmentSession>, AppointmentSessionMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
