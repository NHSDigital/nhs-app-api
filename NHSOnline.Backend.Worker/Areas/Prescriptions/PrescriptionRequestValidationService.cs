using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public class PrescriptionRequestValidationService : IPrescriptionRequestValidationService
    {
        public bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            return fromDate != null && fromDate > defaultFromDate && fromDate < DateTimeOffset.Now;
        }
        
        public bool IsValidRepeatPrescriptionRequest(RepeatPrescriptionRequest model)
        {
            return model.CourseIds.Any() && IsValidGuidList(model.CourseIds);
        }

        private static bool IsValidGuidList(IEnumerable<string> courseIds)
        {
            return courseIds.All(courseId => Guid.TryParse(courseId, out var _));
        }
    }
}
