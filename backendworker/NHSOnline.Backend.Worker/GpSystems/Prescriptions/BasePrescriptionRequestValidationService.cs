using System;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public abstract class BasePrescriptionRequestValidationService : IPrescriptionRequestValidationService
    {
        public virtual bool IsValidRepeatPrescriptionRequest(RepeatPrescriptionRequest model)
        {
            return model.CourseIds.Any() && IsValidSpecialRequest(model.SpecialRequest);
        }

        public bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            return fromDate != null && fromDate > defaultFromDate && fromDate < DateTimeOffset.Now;
        }

        protected static bool IsValidSpecialRequest(string specialRequest)
        {
            return specialRequest == null || specialRequest.Length <= 1000;
        }
    }
}
