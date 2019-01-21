using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

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

        protected override bool IsSupplierPostValid(RepeatPrescriptionRequest request)
        {
            return IsValidGuidList(request.CourseIds);
        }

        private static bool IsValidGuidList(IEnumerable<string> courseIds)
        {
            return courseIds.All(courseId => Guid.TryParse(courseId, out _));
        }
    }
}