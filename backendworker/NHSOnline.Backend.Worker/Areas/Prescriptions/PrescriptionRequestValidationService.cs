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
            return model.CourseIds.Any() 
                && IsValidGuidList(model.CourseIds)
                && IsValidSpecialRequest(model.SpecialRequest);
        }

        private static bool IsValidGuidList(IEnumerable<string> courseIds)
        {
            return courseIds.All(courseId => Guid.TryParse(courseId, out var _));
        }

        private bool IsValidSpecialRequest(string specialRequest)
        {
            return specialRequest == null || specialRequest.Length <= 1000;
        }
    }
}
