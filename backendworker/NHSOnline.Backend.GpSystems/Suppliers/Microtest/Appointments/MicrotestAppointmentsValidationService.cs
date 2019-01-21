using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsValidationService : AppointmentsValidationService
    {
        private readonly ILogger<MicrotestAppointmentsValidationService> _logger;

        public MicrotestAppointmentsValidationService(ILogger<MicrotestAppointmentsValidationService> logger) : base(logger)
        {
            _logger = logger;
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
