using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsValidationService : AppointmentsValidationService
    {
        private readonly ILogger<TppAppointmentsValidationService> _logger;

        public TppAppointmentsValidationService(ILogger<TppAppointmentsValidationService> logger) : base(logger)
        {
            _logger = logger;
        }
        protected override bool IsSupplierDeleteValid(AppointmentCancelRequest request)
        {
            return true;
        }

        protected override bool IsSupplierPostValid(AppointmentBookRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNull(request.StartTime, nameof(request.StartTime))
                .IsNotNull(request.EndTime, nameof(request.EndTime))
                .IsValid();
        }
    }
}
