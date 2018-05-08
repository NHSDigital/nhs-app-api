using System;

namespace NHSOnline.Backend.Worker.Router.Validators
{
    public interface IPrescriptionRequestValidationService
    {
        bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);
    }
}
