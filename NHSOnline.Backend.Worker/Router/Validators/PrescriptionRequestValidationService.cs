using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Router.Validators
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

        private bool IsValidGuidList(IEnumerable<string> courseIds)
        {
            Guid guidOutput;
            bool isValid = true;
                
            foreach (string courseId in courseIds)
            {
                isValid = Guid.TryParse(courseId, out guidOutput);

                if (!isValid)
                {
                    return false;
                }
            }

            return isValid;
        }
    }
}
