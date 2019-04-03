using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentsValidationService : AppointmentsValidationService
    {
        private readonly ILogger<VisionAppointmentsValidationService> _logger;

        public VisionAppointmentsValidationService(ILogger<VisionAppointmentsValidationService> logger) : base(logger)
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
