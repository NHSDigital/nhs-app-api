using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
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