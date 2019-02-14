using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisPrescriptionRequestValidationService : BasePrescriptionRequestValidationService
    {
        public override bool IsValidRepeatPrescriptionRequest(RepeatPrescriptionRequest model)
        {
            return base.IsValidRepeatPrescriptionRequest(model) && IsValidGuidList(model.CourseIds);
        }

        private static bool IsValidGuidList(IEnumerable<string> courseIds)
        {
            return courseIds.All(courseId => Guid.TryParse(courseId, out _));
        }
    }
}