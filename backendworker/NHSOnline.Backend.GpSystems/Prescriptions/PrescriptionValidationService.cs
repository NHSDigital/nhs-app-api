using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IPrescriptionValidationService
    {
        bool IsGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);

        bool IsPostValid(RepeatPrescriptionRequest request, GpUserSession userSession);

        string MassageSpecialRequest(string specialRequest);
    }

    public abstract class PrescriptionValidationService : IPrescriptionValidationService
    {
        private readonly ILogger _logger;

        protected PrescriptionValidationService(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            var baseRequestValid = IsValidFromDate(fromDate, defaultFromDate);

            var supplierSpecificRequestValid = IsSupplierGetValid(fromDate, defaultFromDate);

            return baseRequestValid & supplierSpecificRequestValid;
        }

        protected abstract bool IsSupplierGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);

        public bool IsPostValid(RepeatPrescriptionRequest request, GpUserSession userSession)
        {
            var courseIdsValid = new ValidateAndLog(_logger)
                .IsListPopulated(request.CourseIds, nameof(request.CourseIds))
                .IsSafeString(request.SpecialRequest, nameof(request.SpecialRequest))
                .IsValid();

            var specialRequestValid = IsValidSpecialRequest(request.SpecialRequest);

            var supplierSpecificRequestValid = IsSupplierPostValid(request, userSession);

            return courseIdsValid & specialRequestValid & supplierSpecificRequestValid;
        }

        public string MassageSpecialRequest(string specialRequest)
        {
            if (string.IsNullOrEmpty(specialRequest))
            {
                return specialRequest;
            }

            var encodedSpecialRequestLength = specialRequest.FindNewlineStringEncodedLength(Constants.EncodedCharacterValues.NewLineEncodedValue);

            if (encodedSpecialRequestLength > Constants.SpecialRequestCharacterLimit.BackendLimit)
            {
                // 1 is subtracted as when we first split the string the length becomes 2, as there is text before and after the newline. So by subtracting 1 we get the count of the newlines
                var numberOfLines = specialRequest.Split('\n').Length;
                var numberOfLineTerminators = numberOfLines - 1;

                specialRequest = specialRequest.Replace('\n', ' ');
                _logger.LogFieldCharacterLimitExceeded("Select Medication", "Notes", "Character Limit Exceeded", numberOfLineTerminators);
            }

            return specialRequest;
        }

        protected abstract bool IsSupplierPostValid(RepeatPrescriptionRequest request, GpUserSession gpUserSession);

        private bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            if(fromDate != null && fromDate > defaultFromDate && fromDate < DateTimeOffset.Now)
            {
                return true;
            }

            _logger.LogWarning("Invalid from date supplied");
            return false;
        }

        private  bool IsValidSpecialRequest(string specialRequest)
        {
            if (specialRequest != null && specialRequest.Length > Constants.SpecialRequestCharacterLimit.BackendLimit)
            {
                _logger.LogWarning("Invalid special request");
                return false;
            }
            return true;
        }
    }
}
