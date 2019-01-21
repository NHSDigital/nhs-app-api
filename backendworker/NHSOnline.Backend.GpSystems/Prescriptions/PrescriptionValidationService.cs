using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IPrescriptionValidationService
    {
        bool IsGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);

        bool IsPostValid(RepeatPrescriptionRequest request);
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

        public bool IsPostValid(RepeatPrescriptionRequest request)
        {
            var courseIdsValid = new ValidateAndLog(_logger)
                .IsListPopulated(request.CourseIds, nameof(request.CourseIds))
                .IsSafeString(request.SpecialRequest, nameof(request.SpecialRequest))
                .IsValid();

            var specialRequestValid = IsValidSpecialRequest(request.SpecialRequest);

            var supplierSpecificRequestValid = IsSupplierPostValid(request);

            return courseIdsValid & specialRequestValid & supplierSpecificRequestValid;
        }

        protected abstract bool IsSupplierPostValid(RepeatPrescriptionRequest request);

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
            if (specialRequest != null && specialRequest.Length > 1000)
            {
                _logger.LogWarning("Invalid special request");
                return false;
            }
            return true;
        }
    }
}
