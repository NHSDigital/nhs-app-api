using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsValidationService : AppointmentsValidationService
    {
        private readonly ILogger<EmisAppointmentsValidationService> _logger;

        public EmisAppointmentsValidationService(ILogger<EmisAppointmentsValidationService> logger) : base(logger)
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
