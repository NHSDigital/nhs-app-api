using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentsValidationService
    {
        bool IsDeleteValid(AppointmentCancelRequest request);

        bool IsPostValid(AppointmentBookRequest request);

        string MassageBookingReason(string bookingReason);
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
                .IsStringValidLength(request.BookingReason, 0, Constants.BookingReasonCharacterLimit.BackendLimit, nameof(request.BookingReason))                
                .IsSafeString(request.BookingReason, nameof(request.BookingReason))
                .IsValid();

            var supplierSpecificRequestValid = IsSupplierPostValid(request);

            return baseRequestValid && supplierSpecificRequestValid;
        }

        public string MassageBookingReason(string bookingReason)
        {
            if (string.IsNullOrEmpty(bookingReason))
            {
                return bookingReason;
            }

            var encodeBookingReasonLength =
                bookingReason.FindNewlineStringEncodedLength(Constants.EncodedCharacterValues.NewLineEncodedValue);

            if (encodeBookingReasonLength > Constants.BookingReasonCharacterLimit.BackendLimit)
            {
                // 1 is subtracted as when we first split the string the length becomes 2, as there is text before and after the newline. So by subtracting 1 we get the count of the newline
                var numberOfLines = bookingReason.Split(Environment.NewLine).Length;
                var numberOfLineTerminators = numberOfLines - 1;

                bookingReason = bookingReason.Replace(Environment.NewLine, " ", StringComparison.Ordinal);
                _logger.LogFieldCharacterLimitExceeded("Confirm Appointment", "Reason", "Character Limit Exceeded", numberOfLineTerminators);
            }

            return bookingReason;
        }

        protected abstract bool IsSupplierPostValid(AppointmentBookRequest request);
    }
}
