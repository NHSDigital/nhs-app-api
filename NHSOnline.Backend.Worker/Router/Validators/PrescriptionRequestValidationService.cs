using System;

namespace NHSOnline.Backend.Worker.Router.Validators
{
    public class PrescriptionRequestValidationService : IPrescriptionRequestValidationService
    {
        public bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            return fromDate != null && fromDate > defaultFromDate && fromDate < DateTimeOffset.Now;
        }
    }
}
