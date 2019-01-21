using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentsValidationService
    {
        bool IsDeleteValid(AppointmentCancelRequest request);

        bool IsPostValid(AppointmentBookRequest request);
    }

    public abstract class AppointmentsValidationService : IAppointmentsValidationService
    {
        private readonly ILogger _logger;

        protected AppointmentsValidationService(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsDeleteValid(AppointmentCancelRequest request)
        {
            var baseRequestValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.AppointmentId, nameof(request.AppointmentId))
                .IsValid();

            var supplierSpecificRequestValid = IsSupplierDeleteValid(request);

            return baseRequestValid && supplierSpecificRequestValid;
        }

        protected abstract bool IsSupplierDeleteValid(AppointmentCancelRequest request);

        public bool IsPostValid(AppointmentBookRequest request)
        {
            var baseRequestValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.SlotId, nameof(request.SlotId))
                .IsStringValidLength(request.BookingReason, 0, 150, nameof(request.BookingReason))
                .IsSafeString(request.BookingReason, nameof(request.BookingReason))
                .IsValid();

            var supplierSpecificRequestValid = IsSupplierPostValid(request);

            return baseRequestValid && supplierSpecificRequestValid;
        }

        protected abstract bool IsSupplierPostValid(AppointmentBookRequest request);

    }
}
