using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsValidationService : AppointmentsValidationService
    {
        public MicrotestAppointmentsValidationService(ILogger<MicrotestAppointmentsValidationService> logger) : base(logger)
        {
        }

        protected override bool IsSupplierDeleteValid(AppointmentCancelRequest request)
        {
            return true;
        }

        protected override bool IsSupplierPostValid(AppointmentBookRequest request)
        {
            return true;
        }
    }
}
