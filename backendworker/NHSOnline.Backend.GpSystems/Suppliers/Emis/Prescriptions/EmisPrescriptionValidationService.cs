using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisPrescriptionValidationService : PrescriptionValidationService
    {
        public EmisPrescriptionValidationService(ILogger<EmisPrescriptionValidationService> logger) : base(logger)
        {
        }

        protected override bool IsSupplierGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            return true;
        }

        protected override bool IsSupplierPostValid(RepeatPrescriptionRequest request, GpUserSession gpUserSession)
        {
            var necessity = ((EmisUserSession) gpUserSession).PrescriptionSpecialRequestNecessity;

            var specialRequestIsValid = necessity == Necessity.NotAllowed && request.SpecialRequest is null ||
                                        necessity == Necessity.Mandatory && !string.IsNullOrEmpty(request.SpecialRequest) ||
                                        necessity == Necessity.Optional;

            return IsValidGuidList(request.CourseIds) && specialRequestIsValid;
        }

        private static bool IsValidGuidList(IEnumerable<string> courseIds)
        {
            return courseIds.All(courseId => Guid.TryParse(courseId, out _));
        }
    }
}